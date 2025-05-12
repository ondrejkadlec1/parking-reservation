using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkingReservation.Services.Interfaces;
using ParkingReservation.Services.Results;
using ParkingReservation.Dtos.Reservations;
using ParkingReservation.Dtos.Interfaces;

namespace ParkingReservation.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class BlockingsController(IReservationWriteService writeService, IReservationReadService readService) : ControllerBase
    {
        IReservationWriteService _writeService = writeService;
        IReservationReadService _readService = readService;

        /// <summary>
        /// Vytvoří novou blokaci, pokud už místo není zablokované a zruší konfliktní rezervace.
        /// </summary>
        /// <param name="dto">Reprezentace blokace.</param>
        /// <returns>Reprezentace vytvořené blokace.</returns>

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<BlockingDto>> Post(CreateBlockingDto dto)
        {
            var call = await _writeService.CreateBlocking(dto, User);
            if (!call.Success)
            {
                if (call.ErrorCode == Errors.NotFound)
                {
                    return NotFound($"Místo {dto.SpaceNumber} neexistuje.");
                }
                if (call.ErrorCode == Errors.Conflict)
                {
                    return BadRequest($"Místo {dto.SpaceNumber} již je blokováno.");
                }
            }
            return Ok(call.Object); 
        }

        [HttpGet("my")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<BlockingDto>>> GetMy()
        {
            var call = await _readService.GetFutureBlockingsByUser(User);
            return Ok(call.Object);
        }
    }
}
