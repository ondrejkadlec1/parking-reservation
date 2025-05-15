using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkingReservation.Dtos.Reservations;
using ParkingReservation.Services.BlockingService;

namespace ParkingReservation.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class BlockingsController(IBlockingService service) : ControllerBase
    {

        /// <summary>
        /// Vytvoří novou blokaci, pokud už místo není zablokované a zruší konfliktní rezervace.
        /// </summary>
        /// <param name="dto">Reprezentace blokace.</param>
        /// <returns>Reprezentace vytvořené blokace.</returns>

        [HttpPost]
        [ProducesResponseType(201)]
        public async Task<ActionResult<BlockingResponseDto>> Post(BlockingRequestDto dto)
        {
            return Created("api/Blockings/my", await service.CreateBlocking(dto, User));
        }

        [HttpGet("my")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<BlockingResponseDto>>> GetMy()
        {
            return Ok(await service.GetFutureBlockingsByUser(User));
        }
    }
}
