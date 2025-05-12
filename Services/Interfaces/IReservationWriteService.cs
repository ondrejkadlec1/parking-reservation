using ParkingReservation.Services.Results;
using ParkingReservation.Dtos.Reservations;
using System.Security.Claims;

namespace ParkingReservation.Services.Interfaces
{
    public interface IReservationWriteService
    {
        public Task<ServiceCallResult<ReservationDto>> Create(ReservationRequestDto dto, ClaimsPrincipal user);
        public Task<ServiceCallResult<BlockingDto>> CreateBlocking(CreateBlockingDto dto, ClaimsPrincipal user);
        public Task<ServiceCallResult> ConfirmRequest(Guid id);
        public Task<ServiceCallResult> ConfirmAllRequests();
        public Task<ServiceCallResult> CancelReservation(Guid id, ClaimsPrincipal user);
    }
}
