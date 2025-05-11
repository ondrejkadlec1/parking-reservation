using ParkingReservation.Services.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkingReservation.Dtos.Reservations;
using ParkingReservation.Services.Interfaces;
using Microsoft.Graph.Models;

namespace ParkingReservation.Controllers
{
    /// <summary>
    /// Operace s rezervacemi.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController(IReservationReadService readService, IReservationWriteService writeService) : ControllerBase
    {
        IReservationReadService _readService = readService;
        IReservationWriteService _writeService = writeService;

        /// <summary>
        /// Vrátí všechny rezervace s nerozhodutým stavem.
        /// </summary>
        /// <returns>Reprezentace nerozhodnutých rezervací.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("requests")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<ReservationDto>>> GetRequests()
        {
            var call = await _readService.GetFutureRequests();
            return Ok(call.Object);
        }

        /// <summary>
        /// Vrátí všechny rezervace přihlášeného uživatele.
        /// </summary>
        /// <returns>Reprezentace rezervací uživatele.</returns>
        [HttpGet("my")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<ReservationDto>>> GetMy()
        {
            var call = await _readService.GetByUser(User);
            return Ok(call.Object);
        }

        /// <summary>
        /// Vrátí všechny rezervace daného místa.
        /// </summary>
        /// <returns>Reprezentace rezervací místa.</returns>
        [HttpGet("{spaceNumber}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<ReservationDto>>> GetBySpace(int spaceNumber)
        {
            var call = await _readService.GetFutureBySpace(spaceNumber);
            return Ok(call.Object);
        }

        /// <summary>
        /// Požádá o rezervaci, pokud existuje volné místo.
        /// </summary>
        /// <param name="request">Reprezentace požadavku na rezervaci.</param>
        /// <returns>Reprezentace vytvořenéhé rezervace.</returns>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ReservationDto>> PostRequest(ReservationRequestDto request)
        {
            var call = await _writeService.Create(request, User);
            if (!call.Success)
            {
                if (call.ErrorCode == Errors.NotFound)
                {
                    return BadRequest($"Všechna místo jsou již rezervována.");
                }
            }
            return Created("api/Reservations/my", call.Object); 
        }

        /// <summary>
        /// Potvrdí rezervaci, pokud existuje.
        /// </summary>
        /// <param name="id">Číslo rezervace</param>

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Confirm(Guid id)
        {
            var call = await _writeService.ConfirmRequest(id);
            if (!call.Success)
            {
                if (call.ErrorCode == Errors.NotFound)
                {
                    return NotFound($"Rezervace {id} neexistuje.");
                }
                if (call.ErrorCode == Errors.InvalidState)
                {
                    return BadRequest($"Rezervaci {id} nelze potvrdit.");
                }
            }
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
            await _writeService.ConfirmAllRequests();
            return NoContent();
        }

        /// <summary>
        /// Zruší rezervaci.
        /// </summary>
        /// <param name="id">Číslo rezervace ke zrušení.</param>

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Cancel(Guid id)
        {
            var call = await _writeService.CancelReservation(id, User);
            if (!call.Success)
            {
                if (call.ErrorCode == Errors.NotFound)
                {
                    return NotFound($"Rezervace {id} neexistuje.");
                }
                if (call.ErrorCode == Errors.Unauthorized)
                {
                    return Forbid();
                }
            }
            return NoContent();
        }
    }
}
