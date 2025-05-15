using System.Security.Claims;
using ParkingReservation.Dtos.Reservations;

namespace ParkingReservation.Services.ReservationService
{
    public interface IReservationWriteService
    {
        public Task<ReservationResponseDto> Create(ReservationRequestDto dto, ClaimsPrincipal user);
        public Task ConfirmRequest(Guid id);
        public Task ConfirmAllRequests();
        public Task CancelReservation(Guid id, ClaimsPrincipal user);
    }
}
