using System.Security.Claims;
using AutoMapper;
using ParkingReservation.Services.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ParkingReservation.Data;
using ParkingReservation.Dtos.Reservations;
using ParkingReservation.Models;
using ParkingReservation.Services.Interfaces;
using Microsoft.Identity.Web;

namespace ParkingReservation.Services
{
    public class ReservationsWriteService(AppDbContext context, IMapper mapper, IAuthorizationService authorizationService): IReservationWriteService
    {
        IAuthorizationService _authorizationService = authorizationService;
        IMapper _mapper = mapper;
        AppDbContext _context = context;
        public async Task<ServiceCallResult> CancelReservation(Guid id, ClaimsPrincipal user)
        {
            var reservation = _context.Reservations.FirstOrDefault(p => p.Id == id);
            if (reservation == null)
            {
                return new ServiceCallResult() { ErrorCode = Errors.NotFound };
            }
            var policy = reservation.TypeId == 1 ? "OwnerOrAdmin" : "Owner";
            var authorization = await _authorizationService.AuthorizeAsync(user, reservation, policy);
            if (!authorization.Succeeded)
            {
                return new ServiceCallResult() { ErrorCode = Errors.Unauthorized };
            }
            if (reservation.EndsAt < DateTime.UtcNow)
            {
                return new ServiceCallResult() { ErrorCode = Errors.InvalidState, Message = "Nelze zrušit rezervaci po skončení." };
            }
            if (reservation.StateId == 3)
            {
                return new ServiceCallResult() { ErrorCode = Errors.InvalidState, Message = "Rezervace už je zrušená." };
            }
            reservation.StateId = 3;
            await _context.SaveChangesAsync();

            return new ServiceCallResult { Success = true };
        }

        public async Task<ServiceCallResult> ConfirmAllRequests()
        {
            await _context.Reservations.Where(p => p.StateId == 1 && p.BeginsAt >= DateTime.UtcNow)
                .ExecuteUpdateAsync(setters => setters.SetProperty(p => p.StateId, 2));
            return new ServiceCallResult() { Success = true };
        }

        public async Task<ServiceCallResult> ConfirmRequest(Guid id)
        {
            await using (var transaction = await context.Database.BeginTransactionAsync()) 
            { 
                var request = await _context.Reservations.FirstOrDefaultAsync(p => p.Id == id);
                if (request == null)
                {
                    return new ServiceCallResult { ErrorCode = Errors.NotFound };
                }
                if (request.StateId == 2)
                {
                    return new ServiceCallResult { 
                        ErrorCode = Errors.InvalidState,
                        Message = $"Rezervace {id} už je potvrzena."
                    };
                }
                if (request.BeginsAt < DateTime.UtcNow || request.StateId != 1)
                {
                    return new ServiceCallResult { 
                        ErrorCode = Errors.InvalidState, 
                        Message = $"Rezervaci {id} nelze potvrdit."
                    };
                }

                request.StateId = 2;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ServiceCallResult { Success = true };
            }
        }

        public async Task<ServiceCallResult<ReservationDto>> Create(ReservationRequestDto dto, ClaimsPrincipal user)
        {
            await using (var transaction = await context.Database.BeginTransactionAsync())
            {
                int? available= await _context.Spaces
                    .Where(p => !p.Reservations
                        .Where(r => r.BeginsAt < dto.EndsAt && r.EndsAt > dto.BeginsAt && r.StateId != 3).Any()
                )
                .Select(p => p.SpaceNumber)
                    .FirstOrDefaultAsync();
                Console.WriteLine(available);
                if (available == null || available == 0)
                {
                    return new ServiceCallResult<ReservationDto> { ErrorCode = Errors.NotFound };
                }
                var reservation = _mapper.Map<Reservation>(dto);
                reservation.SpaceNumber = (int)available;
                reservation.TypeId = 1;
                reservation.StateId = 1;
                reservation.UserId = user.GetObjectId();
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                await _context.Entry(reservation).Reference(p => p.State).LoadAsync();

                await transaction.CommitAsync();

                var result = _mapper.Map<ReservationDto>(reservation);
                result.User = user.GetDisplayName();
                return new ServiceCallResult<ReservationDto>
                {
                    Object = result,
                    Success = true
                };
            }

        }

        public async Task<ServiceCallResult<BlockingDto>> CreateBlocking(CreateBlockingDto dto, ClaimsPrincipal user)
        {
            await using (var transaction = await context.Database.BeginTransactionAsync())
            {
                var space = await _context.Spaces.FirstOrDefaultAsync(p => p.SpaceNumber == dto.SpaceNumber);
                if (space == null)
                {
                    return new ServiceCallResult<BlockingDto> { ErrorCode = Errors.NotFound };
                }
                var conflicts = await _context.Reservations
                    .Where(r => r.BeginsAt < dto.EndsAt && r.EndsAt > dto.BeginsAt && r.StateId != 3)
                    .ToListAsync();
                if (conflicts.Where(p => p.TypeId == 2).Any())
                {
                    return new ServiceCallResult<BlockingDto> { ErrorCode = Errors.Conflict };
                }

                foreach (var conflict in conflicts)
                {
                    conflict.StateId = 3;
                }
                await _context.SaveChangesAsync();

                var blocking = _mapper.Map<Reservation>(dto);
                blocking.TypeId = 2;
                blocking.StateId = 2;
                blocking.UserId = user.GetObjectId();
                _context.Add(blocking);

                await _context.SaveChangesAsync();
                await _context.Entry(blocking).Reference(p => p.State).LoadAsync();

                await transaction.CommitAsync();

                var result = _mapper.Map<BlockingDto>(blocking);
                result.User = user.GetDisplayName();
                return new ServiceCallResult<BlockingDto>
                {
                    Object = result,
                    Success = true
                };
            }
        }
    }
}
