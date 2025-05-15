using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkingReservation.Dtos.Reservations;
using ParkingReservation.Services.UserService;

namespace ParkingReservation.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserService service) : ControllerBase
    {

        /// <summary>
        /// Synchronizuje databázi uživatelů.
        /// </summary>

        //Později by se akce mohla provádět na pozadí v pravidelných intervalech.
        [HttpPost("synchronize")]
        [ProducesResponseType(204)]
        public async Task<ActionResult<BlockingResponseDto>> Post()
        {
            await service.Synchronize();
            return NoContent();
        }
    }
}
