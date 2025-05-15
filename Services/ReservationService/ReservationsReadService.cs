using System.Security.Claims;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using ParkingReservation.Data;
using ParkingReservation.Dtos.Interfaces;
using ParkingReservation.Dtos.Reservations;
using ParkingReservation.Models;

namespace ParkingReservation.Services.ReservationService
{
    public class ReservationsReadService(
        IMapper mapper,
        AppDbContext context) : IReservationReadService
    {
        private IReservationDto Discriminate(Reservation input)
        {
            if (input.TypeId == 2)
            {
                return mapper.Map<BlockingResponseDto>(input);
            }
            return mapper.Map<ReservationResponseDto>(input);
        }

        public async Task<ICollection<IReservationDto>> GetFutureBySpace(int spaceNumber)
        {
            if (await context.Spaces.FirstOrDefaultAsync(p => p.SpaceNumber == spaceNumber) == null)
            {
                throw new BadHttpRequestException($"Místo {spaceNumber} nexistuje.", StatusCodes.Status400BadRequest);
            }
            var result = await context.Reservations
                .Where(p => p.SpaceNumber == spaceNumber && p.StateId == 2
                    && p.EndsAt >= DateTime.UtcNow)
                .OrderBy(r => r.CreatedAt)
                .Include(r => r.State)
                .Include(r => r.User)
                .ToListAsync();

            var output = result.Select(Discriminate).ToList();
            return output;
        }

        public async Task<ICollection<ReservationResponseDto>> GetNormalByUser(ClaimsPrincipal user)
        {
            var userId = user.GetObjectId();
            var result = await context.Reservations
                .Where(p => p.UserId.ToString() == userId && p.TypeId == 1)
                .OrderBy(r => r.StateId)
                .ThenByDescending(r => r.CreatedAt)
                .Include(r => r.State)
                .Include(r => r.User)
                .Select(p => mapper.Map<ReservationResponseDto>(p))
                .ToListAsync();

            return result;
        }

        public async Task<ICollection<BlockingResponseDto>> GetFutureBlockingsByUser(ClaimsPrincipal user)
        {
            var userId = user.GetObjectId();
            var result = await context.Reservations
                .Where(p => p.UserId.ToString() == userId &&
                    p.TypeId == 2 &&
                    p.StateId == 2 &&
                    p.EndsAt >= DateTime.UtcNow)
                .Include(p => p.State)
                .Include(p => p.User)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => mapper.Map<BlockingResponseDto>(p))
                .ToListAsync();

            return result;
        }

        public async Task<ICollection<ReservationResponseDto>> GetFutureRequests()
        {
            var result = await context.Reservations
                .Where(p => p.StateId == 1
                    && p.BeginsAt >= DateTime.UtcNow)
                .Include(p => p.State)
                .Include(p => p.User)
                .Select(p => mapper.Map<ReservationResponseDto>(p))
                .ToListAsync();

            return result;
        }

        public bool OwnsReservation(ClaimsPrincipal user, Reservation reservation)
        {
            var userId = user.GetObjectId();
            return userId != null && userId == reservation.UserId.ToString();
        }
    }
}
