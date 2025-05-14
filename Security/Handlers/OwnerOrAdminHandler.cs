using Microsoft.AspNetCore.Authorization;
using ParkingReservation.Models;
using ParkingReservation.Security.Requirements;
using ParkingReservation.Services.Interfaces;

namespace ParkingReservation.Security.Handlers
{
    public class OwnerOrAdminHandler : AuthorizationHandler<OwnerOrAdminRequirement, Reservation>
    {
        private readonly IReservationReadService _service;
        public OwnerOrAdminHandler(IReservationReadService service)
        {
            _service = service;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerOrAdminRequirement requirement, Reservation resource)
        {
            ;
            var isAdmin = context.User.IsInRole("Admin");
            if (isAdmin || _service.OwnsReservation(context.User, resource))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
