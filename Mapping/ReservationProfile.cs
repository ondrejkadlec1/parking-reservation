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
            CreateMap<Reservation, IReservationDto>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.User.DisplayName));
            CreateMap<Reservation, ReservationResponseDto>()
                .IncludeBase<Reservation, IReservationDto>()
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State.NameCs))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src =>
                    src.StateId != 3 && src.EndsAt > DateTime.UtcNow));
            CreateMap<ReservationRequestDto, Reservation>();
            CreateMap<Reservation, BlockingResponseDto>()
                .IncludeBase<Reservation, IReservationDto>();
            CreateMap<BlockingRequestDto, Reservation>();
        }

    }
}
