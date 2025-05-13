using System.Collections.ObjectModel;
using System.Security.Claims;
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
        AppDbContext _context = context;
        IMapper _mapper = mapper;

        /// <summary>
        /// Vrátí všechna parkovací místa.
        /// </summary>
        /// <returns>Reprezentace všech parkovacích míst.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<SpaceDto>>> Get()
        {
            var result = await _context.Spaces.Select(p => _mapper.Map<SpaceDto>(p)).ToListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Vrátí obsazenost parkoviště v daném čase.
        /// </summary>
        /// <param name="from">Začátek potenciální rezervace.</param>
        /// <param name="till">Konec potenciální rezervace.</param>
        /// <returns>Počet míst a obscazených míst.</returns>
        [HttpGet("avialible")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Availability>> GetAvailability(
            DateTime from, 
            DateTime till)
        {
            if (from > till || from < DateTime.UtcNow)
            {
                return BadRequest("Zadán neplatný vstup.");
            }

            int total = await _context.Spaces.CountAsync();
            int occupied = await _context.Spaces
                .Include(p => p.Reservations)
                .Where(p => p.Reservations
                    .Where(r => r.StateId != 3 && r.BeginsAt < till && r.EndsAt > from).Any()
                ).CountAsync();
            bool is_avialible = total > occupied;

            var result = new Availability
            {
                OccupiedCount = occupied,
                TotalCount = total,
                Avialible = is_avialible
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
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<SpaceDto>>> Post([FromBody] CreateSpacesDto dto)
        {
            var result = new Collection<Space>();
            var userId = User.GetObjectId();
            for (var i = 0; i < dto.Count; i++)
            {
                var space = new Space { CreatedBy =  userId };
                result.Add(space);
                _context.Add(space);
            }
            await _context.SaveChangesAsync();
            var output = result.Select(_mapper.Map<SpaceDto>);

            return Ok(output);
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
            if (await _context.Spaces.Where(p => p.SpaceNumber == spaceNumber).FirstOrDefaultAsync() == null)
            {
                return NotFound($"Parkovací místo s číslem {spaceNumber} neexistuje.");
            }
            await _context.Spaces.Where(p => p.SpaceNumber == spaceNumber).ExecuteDeleteAsync();
            return NoContent();
        }
    }
}
