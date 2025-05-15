using Microsoft.AspNetCore.Authorization;
using ParkingReservation.Models;
using ParkingReservation.Security.Requirements;
using ParkingReservation.Services.OwnershipService;

namespace ParkingReservation.Security.Handlers
{
    public class OwnerOrAdminHandler(IOwnershipService service) : AuthorizationHandler<OwnerOrAdminRequirement, Reservation>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerOrAdminRequirement requirement, Reservation resource)
        {
            var isAdmin = context.User.IsInRole("Admin");
            if (isAdmin || service.Owns(context.User, resource))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
