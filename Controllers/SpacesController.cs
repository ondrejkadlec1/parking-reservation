using System.Collections.ObjectModel;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using ParkingReservation.Data;
using ParkingReservation.Dtos.Spaces;
using ParkingReservation.Models;

namespace ParkingReservation.Controllers
{
    /// <summary>
    /// Operace s parkovacími místy.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SpacesController(AppDbContext context, IMapper mapper) : ControllerBase
    {

        /// <summary>
        /// Vrátí všechna parkovací místa.
        /// </summary>
        /// <returns>Reprezentace všech parkovacích míst.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<SpaceResponseDto>>> Get()
        {
            var result = await context.Spaces.Select(p => mapper.Map<SpaceResponseDto>(p)).ToListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Vrátí obsazenost parkoviště v daném čase.
        /// </summary>
        /// <param name="from">Začátek potenciální rezervace.</param>
        /// <param name="till">Konec potenciální rezervace.</param>
        /// <returns>Počet míst a obscazených míst.</returns>
        [HttpGet("available")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<AvailabilityDto>> GetAvailability(
            DateTime from,
            DateTime till)
        {
            if (from > till || from < DateTime.UtcNow)
            {
                return BadRequest("Zadán neplatný vstup.");
            }

            int total = await context.Spaces.CountAsync();
            int occupied = await context.Spaces
                .Include(p => p.Reservations)
                .Where(p => p.Reservations
                    .Where(r => r.StateId != 3 && r.BeginsAt < till && r.EndsAt > from).Any()
                ).CountAsync();
            bool is_available = total > occupied;

            var result = new AvailabilityDto
            {
                OccupiedCount = occupied,
                TotalCount = total,
                Available = is_available
            };
            return Ok(result);
        }

        /// <summary>
        /// Přidá daný počet parkovacích míst.
        /// </summary>
        /// <param name="dto">Informace o přidávaných místech.</param>
        /// <returns>Reprezentace nově vytvořených míst.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(201)]
        public async Task<ActionResult<IEnumerable<SpaceResponseDto>>> Post([FromBody] SpacesRequestDto dto)
        {
            var result = new Collection<Space>();
            var userId = User.GetObjectId()!;
            for (var i = 0; i < dto.Count; i++)
            {
                var space = new Space { CreatedBy = Guid.Parse(userId) };
                result.Add(space);
                context.Add(space);
            }
            await context.SaveChangesAsync();
            var output = result.Select(mapper.Map<SpaceResponseDto>);

            return Created("api/Spaces", output);
        }

        /// <summary>
        /// Smaže parkovací místo pokud existuje.
        /// </summary>
        /// <param name="spaceNumber">Číslo parkovacího místa ke smazání.</param>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{spaceNumber}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Delete(int spaceNumber)
        {
            if (await context.Spaces.Where(p => p.SpaceNumber == spaceNumber).FirstOrDefaultAsync() == null)
            {
                return NotFound($"Parkovací místo s číslem {spaceNumber} neexistuje.");
            }
            await context.Spaces.Where(p => p.SpaceNumber == spaceNumber).ExecuteDeleteAsync();
            return NoContent();
        }
    }
}
