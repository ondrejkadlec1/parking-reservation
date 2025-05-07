using AutoMapper;
using ParkingReservation.Dtos;
using ParkingReservation.Models;

namespace ParkingReservation.Mapping
{
    public class ReservationProfile : Profile
    {
        public ReservationProfile()
        {
            CreateMap<Reservation, ReservationDto>();
            CreateMap<ReservationRequestDto, Reservation>();
        }

    }
}
