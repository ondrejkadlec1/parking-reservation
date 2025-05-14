using System.Security.Claims;
using ParkingReservation.Dtos.Interfaces;
using ParkingReservation.Models;
using ParkingReservation.Services.Results;

namespace ParkingReservation.Services.Interfaces
{
    public interface IReservationReadService
    {
        public bool OwnsReservation(ClaimsPrincipal user, Reservation reservation);
        public Task<ServiceCallResult<ICollection<IReservationDto>>> GetNormalByUser(ClaimsPrincipal user);
        public Task<ServiceCallResult<ICollection<IReservationDto>>> GetFutureBlockingsByUser(ClaimsPrincipal user);
        public Task<ServiceCallResult<ICollection<IReservationDto>>> GetFutureBySpace(int spaceNumber);
        public Task<ServiceCallResult<ICollection<IReservationDto>>> GetFutureRequests();
    }
}
