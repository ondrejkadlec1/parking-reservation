using System.Security.Claims;
using AutoMapper;
using ParkingReservation.Services.Results;
using ParkingReservation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ParkingReservation.Data;
using ParkingReservation.Dtos.Reservations;
using ParkingReservation.Dtos.Interfaces;
using ParkingReservation.Models;
using Microsoft.Identity.Web;

namespace ParkingReservation.Services
{
    public class ReservationsReadService(AppDbContext context, IMapper mapper, IUserService userService): IReservationReadService
    {
        IMapper _mapper = mapper;
        AppDbContext _context = context;
        IUserService _userService = userService;
        
        internal IReservationDto Discriminate(Reservation input)
        {
            if (input.TypeId == 2)
            {
                return _mapper.Map<BlockingDto>(input);
            }
            return _mapper.Map<ReservationDto>(input);
        }

        internal async Task<ICollection<IReservationDto>> MapWithUsernames(ICollection<Reservation> reservations)
        {
            var userIds = reservations.Select(p => p.UserId).Distinct().ToList();
            var call = await _userService.GetUsernames(userIds);
            var usernames = call.Object;

            var mapped = new List<IReservationDto>();
            foreach (var reservation in reservations) 
            { 
                var dto = Discriminate(reservation);
                dto.User = usernames[reservation.UserId];
                mapped.Add(dto);
            }
            return mapped;
        }

        public async Task<ServiceCallResult<ICollection<IReservationDto>>> GetFutureBySpace(int spaceNumber)
        {
            if (await _context.Spaces.FirstOrDefaultAsync(p => p.SpaceNumber == spaceNumber) == null)
            {
                return new ServiceCallResult<ICollection<IReservationDto>> { ErrorCode = Errors.NotFound };
            }
            var result = await _context.Reservations
                .Where(p => p.SpaceNumber == spaceNumber && p.StateId == 2
                    && p.EndsAt >= DateTime.UtcNow)
                .OrderBy(r => r.CreatedAt)
                .Include(r => r.State)
                .ToListAsync();

            var output = await MapWithUsernames(result);
            return new ServiceCallResult<ICollection<IReservationDto>>
            {
                Object = (ICollection<IReservationDto>)output,
                Success = true
            };
        }

        public async Task<ServiceCallResult<ICollection<IReservationDto>>> GetNormalByUser(ClaimsPrincipal user)
        {
            var userId = user.GetObjectId();
            var result = await _context.Reservations
                .Where(p => p.UserId == userId && p.TypeId == 1)
                .OrderBy(r => r.StateId)
                .ThenByDescending(r => r.CreatedAt)
                .Include(r => r.State)
                .ToListAsync();

            var output = await MapWithUsernames(result);
            return new ServiceCallResult<ICollection<IReservationDto>>
            {
                Object = (ICollection<IReservationDto>)output,
                Success = true
            };
        }

        public async Task<ServiceCallResult<ICollection<IReservationDto>>> GetFutureBlockingsByUser(ClaimsPrincipal user)
        {
            var userId = user.GetObjectId();
            var result = await _context.Reservations
                .Where(p => p.UserId == userId && 
                    p.TypeId == 2 &&
                    p.StateId == 2 &&
                    p.EndsAt >= DateTime.UtcNow)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var output = await MapWithUsernames(result);
            return new ServiceCallResult<ICollection<IReservationDto>>
            {
                Object = (ICollection<IReservationDto>)output,
                Success = true
            };
        }

        public async Task<ServiceCallResult<ICollection<IReservationDto>>> GetFutureRequests()
        {
            var result = await _context.Reservations
                .Where(p => p.StateId == 1
                    && p.BeginsAt >= DateTime.UtcNow)
                .ToListAsync();

            var output = await MapWithUsernames(result);
            return new ServiceCallResult<ICollection<IReservationDto>>
            {
                Object = (ICollection<IReservationDto>)output,
                Success = true
            };
        }

        public bool OwnsReservation(ClaimsPrincipal user, Reservation reservation)
        {
            return user.GetObjectId() == reservation.UserId;
        }
    }
}
