using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using ParkingReservation.Models;
using ParkingReservation.Security.Requirements;
using ParkingReservation.Services.Interfaces;

namespace ParkingReservation.Security.Handlers
{
    public class OwnerOrAdminHandler(IReservationReadService service) : AuthorizationHandler<OwnerOrAdminRequirement, Reservation>
    {
        IReservationReadService _service = service;

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerOrAdminRequirement requirement, Reservation resource)
        {;
            var isAdmin = context.User.IsInRole("Admin");
            if (isAdmin || _service.OwnsReservation(context.User, resource))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
