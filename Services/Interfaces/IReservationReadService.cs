using ParkingReservation.Services.Results;
using ParkingReservation.Models;
using ParkingReservation.Dtos.Reservations;
using System.Security.Claims;

namespace ParkingReservation.Services.Interfaces
{
    public interface IReservationReadService
    {
        public bool OwnsReservation(ClaimsPrincipal user, Reservation reservation);
        public Task<ServiceCallResult<ICollection<ReservationDto>>> GetByUser(ClaimsPrincipal user);
        public Task<ServiceCallResult<ICollection<ReservationDto>>> GetFutureBySpace(int spaceNumber);
        public Task<ServiceCallResult<ICollection<ReservationDto>>> GetFutureRequests();
    }
}
