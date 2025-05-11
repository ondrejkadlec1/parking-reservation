using System.Security.Claims;
using AutoMapper;
using ParkingReservation.Services.Results;
using ParkingReservation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ParkingReservation.Data;
using ParkingReservation.Dtos.Reservations;
using ParkingReservation.Models;

namespace ParkingReservation.Services
{
    public class ReservationsReadService(AppDbContext context, IMapper mapper): IReservationReadService
    {
        IMapper _mapper = mapper;
        AppDbContext _context = context;
        
        public async Task<ServiceCallResult<ICollection<ReservationDto>>> GetFutureBySpace(int spaceNumber)
        {
            var result = await _context.Reservations
                .Where(p => p.SpaceNumber == spaceNumber && p.StateId == 2
                    && p.EndsAt >= DateTime.UtcNow)
                .OrderBy(r => r.IssuedAt)
                .Include(r => r.State)
                .Select(r => _mapper.Map<ReservationDto>(r))
                .ToListAsync();
            return new ServiceCallResult<ICollection<ReservationDto>> { Object = result, Success = true };
        }

        public async Task<ServiceCallResult<ICollection<ReservationDto>>> GetByUser(ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _context.Reservations
                .Where(p => p.UserId == userId)
                .OrderBy(r => r.StateId)
                .ThenByDescending(r => r.IssuedAt)
                .Include(r => r.State)
                .Select(r => _mapper.Map<ReservationDto>(r))
                .ToListAsync();
            return new ServiceCallResult<ICollection<ReservationDto>> { Object = result, Success = true };
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
