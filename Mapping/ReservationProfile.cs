using AutoMapper;
using ParkingReservation.Dtos.Reservations;
using ParkingReservation.Dtos.Interfaces;
using ParkingReservation.Models;

namespace ParkingReservation.Mapping
{
    public class ReservationProfile : Profile
    {
        public ReservationProfile()
        {
            CreateMap<Reservation, IOwnedDto>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.User.DisplayName));
            CreateMap<Reservation, ReservationResponseDto>()
                .IncludeBase<Reservation, IOwnedDto>()
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State.NameCs))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src =>
                    src.StateId != 3 && src.EndsAt > DateTime.UtcNow));
            CreateMap<ReservationRequestDto, Reservation>();
            CreateMap<Reservation, BlockingResponseDto>()
                .IncludeBase<Reservation, IOwnedDto>();
            CreateMap<BlockingRequestDto, Reservation>();
        }

    }
}
