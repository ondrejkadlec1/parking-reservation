using Microsoft.AspNetCore.Authorization;
using ParkingReservation.Models;
using ParkingReservation.Security.Requirements;
using ParkingReservation.Services.Interfaces;

namespace ParkingReservation.Security.Handlers
{
    public class OwnershipHandler : AuthorizationHandler<OwnershipRequirement, Reservation>
    {
        private readonly IReservationReadService _service;
        public OwnershipHandler(IReservationReadService service)
        {
            _service = service;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnershipRequirement requirement, Reservation resource)
        {
            if (_service.OwnsReservation(context.User, resource))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
