using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using ParkingReservation.Data;
using ParkingReservation.Dtos.Reservations;
using ParkingReservation.Models;

namespace ParkingReservation.Services.ReservationService
{
    public class ReservationsWriteService(
        IAuthorizationService authorizationService,
        IMapper mapper,
        AppDbContext context) : IReservationWriteService
    {
        public async Task CancelReservation(Guid id, ClaimsPrincipal user)
        {
            var reservation = context.Reservations.FirstOrDefault(p => p.Id == id);
            if (reservation == null)
            {
                throw new BadHttpRequestException($"Rezervace {id} neexistuje.", StatusCodes.Status404NotFound);
            }
            var policy = reservation.TypeId == 1 ? "OwnerOrAdmin" : "Owner";
            var authorization = await authorizationService.AuthorizeAsync(user, reservation, policy);
            if (!authorization.Succeeded)
            {
                throw new BadHttpRequestException("Přístup odepřen", StatusCodes.Status403Forbidden);
            }
            if (reservation.EndsAt < DateTime.UtcNow)
            {
                throw new BadHttpRequestException($"Rezervaci {id} nelze zrušit.", StatusCodes.Status400BadRequest);
            }
            if (reservation.StateId == 3)
            {
                throw new BadHttpRequestException($"Rezervace {id} už je zrušená.", StatusCodes.Status400BadRequest);
            }
            reservation.StateId = 3;
            await context.SaveChangesAsync();
        }

        public async Task ConfirmAllRequests()
        {
            await context.Reservations.Where(p => p.StateId == 1 && p.BeginsAt >= DateTime.UtcNow)
                .ExecuteUpdateAsync(setters => setters.SetProperty(p => p.StateId, 2));
        }

        public async Task ConfirmRequest(Guid id)
        {
            await using (var transaction = await context.Database.BeginTransactionAsync())
            {
                var request = await context.Reservations.FirstOrDefaultAsync(p => p.Id == id);
                if (request == null)
                {
                    throw new BadHttpRequestException($"Rezervace {id} neexistuje.", StatusCodes.Status404NotFound);
                }
                if (request.StateId == 2)
                {
                    throw new BadHttpRequestException($"Rezervace {id} už je potvrzená.", StatusCodes.Status400BadRequest);
                }
                if (request.BeginsAt < DateTime.UtcNow || request.StateId != 1)
                {
                    throw new BadHttpRequestException($"Rezervaci {id} nelze potvrdit.", StatusCodes.Status400BadRequest);
                }

                request.StateId = 2;
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
        }

        public async Task<ReservationResponseDto> Create(ReservationRequestDto dto, ClaimsPrincipal user)
        {
            await using (var transaction = await context.Database.BeginTransactionAsync())
            {
                int? available = await context.Spaces
                    .Where(p => !p.Reservations
                        .Where(r => r.BeginsAt < dto.EndsAt && r.EndsAt > dto.BeginsAt && r.StateId != 3).Any()
                )
                .Select(p => p.SpaceNumber)
                    .FirstOrDefaultAsync();
                Console.WriteLine(available);
                if (available == null || available == 0)
                {
                    throw new BadHttpRequestException($"Všechna místa jsou obsazena.", StatusCodes.Status404NotFound);
                }
                var reservation = mapper.Map<Reservation>(dto);
                reservation.SpaceNumber = (int)available;
                reservation.TypeId = 1;
                reservation.StateId = 1;
                reservation.UserId = Guid.Parse(user.GetObjectId()!);
                context.Add(reservation);
                await context.SaveChangesAsync();
                await context.Entry(reservation).Reference(p => p.State).LoadAsync();

                await transaction.CommitAsync();

                var result = mapper.Map<ReservationResponseDto>(reservation);
                result.DisplayName = user.GetDisplayName() ?? user.GetObjectId()!;
                return result;
            }

        }

        public async Task<BlockingResponseDto> CreateBlocking(BlockingRequestDto dto, ClaimsPrincipal user)
        {
            await using (var transaction = await context.Database.BeginTransactionAsync())
            {
                var space = await context.Spaces.FirstOrDefaultAsync(p => p.SpaceNumber == dto.SpaceNumber);
                if (space == null)
                {
                    throw new BadHttpRequestException($"Místo {dto.SpaceNumber} neexistuje.", StatusCodes.Status404NotFound);
                }
                var conflicts = await context.Reservations
                    .Where(r => r.BeginsAt < dto.EndsAt && r.EndsAt > dto.BeginsAt && r.StateId != 3)
                    .ToListAsync();
                if (conflicts.Where(p => p.TypeId == 2).Any())
                {
                    throw new BadHttpRequestException($"Místo {dto.SpaceNumber} je zablokováno.", StatusCodes.Status400BadRequest);
                }

                foreach (var conflict in conflicts)
                {
                    conflict.StateId = 3;
                }
                await context.SaveChangesAsync();

                var blocking = mapper.Map<Reservation>(dto);
                blocking.TypeId = 2;
                blocking.StateId = 2;
                blocking.UserId = Guid.Parse(user.GetObjectId()!);
                context.Add(blocking);

                await context.SaveChangesAsync();
                await context.Entry(blocking).Reference(p => p.State).LoadAsync();

                await transaction.CommitAsync();

                var result = mapper.Map<BlockingResponseDto>(blocking);
                result.DisplayName = user.GetDisplayName() ?? user.GetObjectId()!;
                return result;
            }
        }
    }
}
