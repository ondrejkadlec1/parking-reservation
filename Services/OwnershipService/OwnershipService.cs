using Microsoft.Identity.Web;
using ParkingReservation.Models;
using System.Security.Claims;

namespace ParkingReservation.Services.OwnershipService
{
    public class OwnershipService : IOwnershipService
    {
        public bool Owns(ClaimsPrincipal user, Reservation reservation)
        {
            var userId = user.GetObjectId();
            return userId != null && userId == reservation.UserId.ToString();
        }
    }
}
