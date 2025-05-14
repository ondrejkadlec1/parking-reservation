using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using ParkingReservation.Data;

namespace ParkingReservation.Services
{
    public class RoleClaimTransformer : IClaimsTransformation
    {
        private readonly AppDbContext _context;
        public RoleClaimTransformer(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal.Identity == null)
            {
                return principal;
            }
            var identity = (ClaimsIdentity)principal.Identity!;
            var id = principal.GetObjectId();
            if (await _context.Admins.Where(p => p.Id == id).AnyAsync())
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
            }
            return principal;
        }
    }
}
