using Microsoft.AspNetCore.Authorization;
using ParkingReservation.Models;
using ParkingReservation.Security.Requirements;
using ParkingReservation.Services.OwnershipService;

namespace ParkingReservation.Security.Handlers
{
    public class OwnershipHandler(IOwnershipService service) : AuthorizationHandler<OwnershipRequirement, Reservation>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnershipRequirement requirement, Reservation resource)
        {
            if (service.Owns(context.User, resource))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
