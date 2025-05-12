using System.Security.Claims;
using AutoMapper;
using ParkingReservation.Services.Results;
using ParkingReservation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ParkingReservation.Data;
using ParkingReservation.Dtos.Reservations;
using ParkingReservation.Dtos.Interfaces;
using ParkingReservation.Models;

namespace ParkingReservation.Services
{
    public class ReservationsReadService(AppDbContext context, IMapper mapper): IReservationReadService
    {
        IMapper _mapper = mapper;
        AppDbContext _context = context;
        
        internal IReservationDto Discriminate(Reservation input)
        {
            if (input.TypeId == 2)
            {
                return _mapper.Map<BlockingDto>(input);
            }
            return _mapper.Map<ReservationDto>(input);
        }

        public async Task<ServiceCallResult<ICollection<IReservationDto>>> GetFutureBySpace(int spaceNumber)
        {
            if (await _context.Spaces.FirstOrDefaultAsync(p => p.SpaceNumber == spaceNumber) == null)
            {
                return new ServiceCallResult<ICollection<IReservationDto>> { ErrorCode = Errors.NotFound };
            }
            var result = await _context.Reservations
                .Where(p => p.SpaceNumber == spaceNumber && p.StateId == 2
                    && p.EndsAt >= DateTime.UtcNow)
                .OrderBy(r => r.CreatedAt)
                .Include(r => r.State)
                .ToListAsync();
                
            return new ServiceCallResult<ICollection<IReservationDto>> { 
                Object = result.Select(Discriminate).ToList(), 
                Success = true };
        }

        public async Task<ServiceCallResult<ICollection<ReservationDto>>> GetNormalByUser(ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _context.Reservations
                .Where(p => p.UserId == userId && p.TypeId == 1)
                .OrderBy(r => r.StateId)
                .ThenByDescending(r => r.CreatedAt)
                .Include(r => r.State)
                .Select(r => _mapper.Map<ReservationDto>(r))
                .ToListAsync();
            return new ServiceCallResult<ICollection<ReservationDto>> { 
                Object = result, 
                Success = true };
        }

        public async Task<ServiceCallResult<ICollection<BlockingDto>>> GetFutureBlockingsByUser(ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _context.Reservations
                .Where(p => p.UserId == userId && 
                    p.TypeId == 2 &&
                    p.StateId == 2 &&
                    p.EndsAt >= DateTime.UtcNow)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => _mapper.Map<BlockingDto>(r))
                .ToListAsync();
            return new ServiceCallResult<ICollection<BlockingDto>>
            {
                Object = result,
                Success = true
            };
        }

        public async Task<ServiceCallResult<ICollection<ReservationDto>>> GetFutureRequests()
        {
            var result = await _context.Reservations
                .Where(p => p.StateId == 1
                    && p.BeginsAt >= DateTime.UtcNow)
                .Select(r => _mapper.Map<ReservationDto>(r))
                .ToListAsync();
            return new ServiceCallResult<ICollection<ReservationDto>> { Object = result, Success = true };
        }

        public bool OwnsReservation(ClaimsPrincipal user, Reservation reservation)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier) == reservation.UserId;
        }
    }
}
