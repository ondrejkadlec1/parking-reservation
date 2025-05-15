using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using ParkingReservation.Data;

namespace ParkingReservation.Security
{
    public class RoleClaimTransformer(AppDbContext context) : IClaimsTransformation
    {
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal.Identity == null)
            {
                return principal;
            }
            var identity = (ClaimsIdentity)principal.Identity!;
            var id = principal.GetObjectId();
            if (await context.Users
                .Where(p => p.Id.ToString() == id && p.IsAdmin == true).AnyAsync())
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
            }
            return principal;
        }
    }
}
