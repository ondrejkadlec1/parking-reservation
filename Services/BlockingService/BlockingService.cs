using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using ParkingReservation.Data;
using ParkingReservation.Dtos.Reservations;
using ParkingReservation.Models;
using System.Security.Claims;

namespace ParkingReservation.Services.BlockingService
{
    public class BlockingService(IMapper mapper, AppDbContext context) : IBlockingService
    {
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
