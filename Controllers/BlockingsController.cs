using System.Runtime.InteropServices;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingReservation.Data;
using ParkingReservation.Dtos;
using ParkingReservation.Models;

namespace ParkingReservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlockingsController(IMapper mapper, AppDbContext context) : ControllerBase
    {
        IMapper _mapper = mapper;
        AppDbContext _context = context;

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<BlockingDto>> Post(CreateBlockingDto dto)
        {
            var blocking = _mapper.Map<Blocking>(dto);
            _context.Add(blocking);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return NotFound($"Místo {dto.SpaceNumber} neexistuje.");
            }
            return Ok(_mapper.Map<BlockingDto>(blocking)); 
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<BlockingDto>>> Get()
        {
            var result = await _context.Blockings.Select(p => _mapper.Map<BlockingDto>(p)).ToListAsync();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _context.Blockings.Where(p => p.Id == id).ExecuteDeleteAsync();
            return NoContent();
        }
    }
}
