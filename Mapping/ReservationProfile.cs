using AutoMapper;
using ParkingReservation.Dtos.Reservations;
using ParkingReservation.Models;

namespace ParkingReservation.Mapping
{
    public class ReservationProfile : Profile
    {
        public ReservationProfile()
        {
            CreateMap<Reservation, ReservationDto>()
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State.NameCs))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => 
                    src.StateId != 3 && src.EndsAt > DateTime.UtcNow));
            CreateMap<ReservationRequestDto, Reservation>();
            CreateMap<Reservation, BlockingDto>();
            CreateMap<CreateBlockingDto, Reservation>();
        }

    }
}
