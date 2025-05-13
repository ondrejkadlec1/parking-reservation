using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Identity.Web;
using ParkingReservation.Data;

namespace ParkingReservation.Services
{
    public class RoleClaimTransformer(AppDbContext context) : IClaimsTransformation
    {
        AppDbContext _context = context;
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identity = (ClaimsIdentity)principal.Identity;
            var id = principal.GetObjectId();
            if (await _context.Admins.Where(p => p.Id == id).AnyAsync())
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
            }
            return principal;
        }
    }
}
