using ParkingReservation.Dtos.Reservations;
using System.Security.Claims;

namespace ParkingReservation.Services.BlockingService
{
    public interface IBlockingService
    {
        public Task<BlockingResponseDto> CreateBlocking(BlockingRequestDto dto, ClaimsPrincipal user);
        public Task<ICollection<BlockingResponseDto>> GetFutureBlockingsByUser(ClaimsPrincipal user);
    }
}
