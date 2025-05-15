using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkingReservation.Dtos.Interfaces;
using ParkingReservation.Dtos.Reservations;
using ParkingReservation.Services.ReservationService;

namespace ParkingReservation.Controllers
{
    /// <summary>
    /// Operace s rezervacemi.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController(IReservationReadService readService, IReservationWriteService writeService) : ControllerBase
    {

        /// <summary>
        /// Vrátí všechny rezervace s nerozhodutým stavem.
        /// </summary>
        /// <returns>Reprezentace nerozhodnutých rezervací.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("requests")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<ReservationResponseDto>>> GetRequests()
        {
            return Ok(await readService.GetFutureRequests());
        }

        /// <summary>
        /// Vrátí všechny rezervace přihlášeného uživatele.
        /// </summary>
        /// <returns>Reprezentace rezervací uživatele.</returns>
        [HttpGet("my")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<ReservationResponseDto>>> GetMy()
        {
            return Ok(await readService.GetNormalByUser(User));
        }

        /// <summary>
        /// Vrátí všechny rezervace daného místa.
        /// </summary>
        /// <returns>Reprezentace rezervací místa.</returns>
        [HttpGet("{spaceNumber}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<IReservationDto>>> GetBySpace(int spaceNumber)
        {
            return Ok(await readService.GetFutureBySpace(spaceNumber));
        }

        /// <summary>
        /// Požádá o rezervaci, pokud existuje volné místo.
        /// </summary>
        /// <param name="request">Reprezentace požadavku na rezervaci.</param>
        /// <returns>Reprezentace vytvořenéhé rezervace.</returns>
        [HttpPost]
        [ProducesResponseType(201)]
        public async Task<ActionResult<ReservationResponseDto>> PostRequest(ReservationRequestDto request)
        {
            var newReservation = await writeService.Create(request, User);
            return Created("api/Reservations/my", newReservation);
        }

        /// <summary>
        /// Potvrdí rezervaci.
        /// </summary>
        /// <param name="id">Číslo rezervace</param>

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        [ProducesResponseType(204)]
        public async Task<ActionResult> Confirm(Guid id)
        {
            await writeService.ConfirmRequest(id);
            return NoContent();
        }

        /// <summary>
        /// Potvrdí všechny nerozhodnuté rezervace.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPatch("confirm-all")]
        [ProducesResponseType(204)]
        public async Task<ActionResult> ConfirmAll()
        {
            await writeService.ConfirmAllRequests();
            return NoContent();
        }

        /// <summary>
        /// Zruší rezervaci.
        /// </summary>
        /// <param name="id">Číslo rezervace ke zrušení.</param>

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        public async Task<ActionResult> Cancel(Guid id)
        {
            await writeService.CancelReservation(id, User);
            return NoContent();
        }
    }
}
