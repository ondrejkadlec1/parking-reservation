using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using ParkingReservation.Models;
using ParkingReservation.Security.Requirements;
using ParkingReservation.Services.Interfaces;

namespace ParkingReservation.Security.Handlers
{
    public class OwnershipHandler(IReservationReadService service): AuthorizationHandler<OwnershipRequirement, Reservation>
    {
        IReservationReadService _service = service;

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
