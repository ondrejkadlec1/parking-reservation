using System.Security.Claims;
using ParkingReservation.Dtos.Reservations;
using ParkingReservation.Dtos.Interfaces;
using ParkingReservation.Models;

namespace ParkingReservation.Services.ReservationService
{
    public interface IReservationReadService
    {
        public bool OwnsReservation(ClaimsPrincipal user, Reservation reservation);
        public Task<ICollection<ReservationResponseDto>> GetNormalByUser(ClaimsPrincipal user);
        public Task<ICollection<BlockingResponseDto>> GetFutureBlockingsByUser(ClaimsPrincipal user);
        public Task<ICollection<IReservationDto>> GetFutureBySpace(int spaceNumber);
        public Task<ICollection<ReservationResponseDto>> GetFutureRequests();
    }
}
