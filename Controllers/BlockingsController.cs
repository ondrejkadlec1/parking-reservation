using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkingReservation.Dtos.Blockings;
using ParkingReservation.Services.Interfaces;
using ParkingReservation.Services.Results;

namespace ParkingReservation.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class BlockingsController(IReservationWriteService service) : ControllerBase
    {
        IReservationWriteService _service = service;

        /// <summary>
        /// Vytvoří novou blokaci, pokud už místo není zablokované a zruší konfliktní rezervace.
        /// </summary>
        /// <param name="dto">Reprezentace blokace.</param>
        /// <returns>Reprezentace vytvořené blokace.</returns>

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<BlockingDto>> Post(CreateBlockingDto dto)
        {
            var call = await _service.CreateBlocking(dto, User);
            if (!call.Success)
            {
                if (call.ErrorCode == Errors.NotFound)
                {
                    return NotFound($"Místo {dto.SpaceNumber} neexistuje.");

                }
            }
            return Ok(call.Object); 
        }
    }
}
