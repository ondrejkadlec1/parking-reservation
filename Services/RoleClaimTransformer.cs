using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Azure;

namespace ParkingReservation.Services
{
    public class RoleClaimTransformer : IClaimsTransformation
    {
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identity = (ClaimsIdentity)principal.Identity;
            identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
            return principal;
        }
    }
}
