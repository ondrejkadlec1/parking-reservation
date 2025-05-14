using Microsoft.AspNetCore.Authorization;

namespace ParkingReservation.Security.Requirements
{
    public class OwnerOrAdminRequirement : IAuthorizationRequirement
    {
    }
}
