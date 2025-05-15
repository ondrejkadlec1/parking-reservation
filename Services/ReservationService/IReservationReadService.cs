using System.Security.Claims;
using ParkingReservation.Dtos.Reservations;
using ParkingReservation.Dtos.Interfaces;
using ParkingReservation.Models;

namespace ParkingReservation.Services.ReservationService
{
    public interface IReservationReadService
    {
        public Task<ICollection<ReservationResponseDto>> GetByUser(ClaimsPrincipal user);
        public Task<ICollection<IReservationDto>> GetFutureBySpace(int spaceNumber);
        public Task<ICollection<ReservationResponseDto>> GetFutureRequests();
    }
}
