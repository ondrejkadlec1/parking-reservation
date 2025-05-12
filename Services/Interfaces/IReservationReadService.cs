using ParkingReservation.Services.Results;
using ParkingReservation.Models;
using ParkingReservation.Dtos.Reservations;
using ParkingReservation.Dtos.Interfaces;
using System.Security.Claims;

namespace ParkingReservation.Services.Interfaces
{
    public interface IReservationReadService
    {
        public bool OwnsReservation(ClaimsPrincipal user, Reservation reservation);
        public Task<ServiceCallResult<ICollection<ReservationDto>>> GetNormalByUser(ClaimsPrincipal user);
        public Task<ServiceCallResult<ICollection<BlockingDto>>> GetFutureBlockingsByUser(ClaimsPrincipal user);
        public Task<ServiceCallResult<ICollection<IReservationDto>>> GetFutureBySpace(int spaceNumber);
        public Task<ServiceCallResult<ICollection<ReservationDto>>> GetFutureRequests();
    }
}
