using ParkingReservation.Models;
using System.Security.Claims;

namespace ParkingReservation.Services.OwnershipService
{
    public interface IOwnershipService
    {
        public bool Owns(ClaimsPrincipal user, Reservation reservation);
    }
}
