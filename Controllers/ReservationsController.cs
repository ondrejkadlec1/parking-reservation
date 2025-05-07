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
    public class ReservationsController(IMapper mapper, AppDbContext context) : ControllerBase
    {
        IMapper _mapper = mapper;
        AppDbContext _context = context;


        [HttpGet("requests")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<ReservationDto>>> GetRequests(int spaceNumber)
        {
            var result = await _context.Reservations
                .Where(p => p.StateId == 1)
                .Select(r => _mapper.Map<ReservationDto>(r))
                .ToListAsync();
            return Ok(result);
        }


        [HttpGet("my")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<ReservationDto>>> GetMy(int spaceNumber)
        {
            var result = await _context.Reservations
                //.Where(p => p.UserId == "kkrdb")
                .OrderBy(r => r.StateId)
                .Select(r => _mapper.Map<ReservationDto>(r))
                .ToListAsync();
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ReservationDto>> PostNew(ReservationRequestDto request)
        {
            await using var transaction = await context.Database.BeginTransactionAsync();

            int? avialible = await _context.Spaces
                .Where(p => !p.Reservations
                    .Where(r => r.BeginsAt < request.EndsAt & r.EndsAt > request.BeginsAt).Any()
                )
                .Select(p => p.SpaceNumber)
                .FirstOrDefaultAsync();

            if (avialible == null) {
                await transaction.RollbackAsync();
                return BadRequest($"Všechna místo jsou již rezervována.");
            }
            var newReservation = _mapper.Map<Reservation>(request);
            newReservation.SpaceNumber = (int)avialible;
            newReservation.StateId = 1;
            newReservation.UserId = "iugzkiuoi";
            _context.Add(newReservation);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return Created("api/Reservations", _mapper.Map<ReservationDto>(newReservation)); 
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Confirm(Guid id)
        {
            var stub = new Reservation { Id = id };
            _context.Attach(stub);
            _context.Entry(stub).Property(p => p.StateId).CurrentValue = 2;
            _context.Entry(stub).Property(p => p.StateId).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
            } catch (DbUpdateException)
            {
                return NotFound($"Rezervace {id} neexistuje.");
            }
            return NoContent();
        }

        [HttpPatch("confirm-all")]
        [ProducesResponseType(204)]
        public async Task<ActionResult> ConfirmAll(Guid id)
        {
            await _context.Reservations.Where(p => p.StateId == 1)
                .ExecuteUpdateAsync(setters => setters.SetProperty(p => p.StateId, 2));
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Cancel(Guid id)
        {
            var stub = new Reservation { Id = id };
            _context.Attach(stub);
            _context.Entry(stub).Property(p => p.StateId).CurrentValue = 3;
            _context.Entry(stub).Property(p => p.StateId).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return NotFound($"Rezervace {id} neexistuje.");
            }
            return NoContent();
        }
    }
}
