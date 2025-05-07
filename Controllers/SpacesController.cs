using System.Collections.ObjectModel;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingReservation.Data;
using ParkingReservation.Dtos;
using ParkingReservation.Models;

namespace ParkingReservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpacesController(AppDbContext context, IMapper mapper) : ControllerBase
    {
        AppDbContext _context = context;
        IMapper _mapper = mapper;

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<SpaceDto>>> GetAll()
        {
            var result = await _context.Spaces.Select(p => _mapper.Map<SpaceDto>(p)).ToListAsync();
            return Ok(result);
        }

        [HttpGet("{spaceNumber}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<SpaceDetailDto>> GetBySpaceNumber(int spaceNumber)
        {
            var result = _mapper.Map<SpaceDetailDto>(await _context.Spaces
                .Where(p => p.SpaceNumber == spaceNumber)
                .Include(p => p.Reservations)
                .FirstOrDefaultAsync());
            return Ok(result);
        }

        [HttpGet("avialible")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<OccupiedSpacesDto>> GetAvialible(DateTime from, DateTime till)
        {
            int total = await _context.Spaces.CountAsync();
            int occupied = await _context.Spaces
                .Where(p => p.Reservations
                    .Where(r => r.BeginsAt < from & r.EndsAt > till).Any()
                ).CountAsync();
            bool is_avialible = total > occupied;

            var result = new OccupiedSpacesDto
            {
                OccupiedCount = occupied,
                TotalCount = total,
                Avialible = is_avialible
            };
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<SpaceDto>>> Post([FromBody] CreateSpacesDto dto)
        {
            var result = new Collection<Space>();
            for (var i = 0; i < dto.Count; i++)
            {
                var space = new Space { CreatedBy = "someusersId" };
                result.Add(space);
                _context.Add(space);
            }
            await _context.SaveChangesAsync();
            var output = result.Select(p => _mapper.Map<SpaceDto>(p));

            return Ok(output);
        }

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
