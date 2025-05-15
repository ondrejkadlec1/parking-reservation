using Microsoft.AspNetCore.Authorization;
using ParkingReservation.Models;
using ParkingReservation.Security.Requirements;
using ParkingReservation.Services.ReservationService;

namespace ParkingReservation.Security.Handlers
{
    public class OwnershipHandler(IReservationReadService service) : AuthorizationHandler<OwnershipRequirement, Reservation>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnershipRequirement requirement, Reservation resource)
        {
            if (service.OwnsReservation(context.User, resource))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
