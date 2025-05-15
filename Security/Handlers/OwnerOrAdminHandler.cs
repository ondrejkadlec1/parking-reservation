using Microsoft.AspNetCore.Authorization;
using ParkingReservation.Models;
using ParkingReservation.Security.Requirements;
using ParkingReservation.Services.ReservationService;

namespace ParkingReservation.Security.Handlers
{
    public class OwnerOrAdminHandler(IReservationReadService service) : AuthorizationHandler<OwnerOrAdminRequirement, Reservation>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerOrAdminRequirement requirement, Reservation resource)
        {
            var isAdmin = context.User.IsInRole("Admin");
            if (isAdmin || service.OwnsReservation(context.User, resource))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
