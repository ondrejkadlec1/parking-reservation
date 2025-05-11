using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace ParkingReservation.Security.Requirements
{
    public class OwnerOrAdminRequirement : IAuthorizationRequirement 
    { 
    }
}
