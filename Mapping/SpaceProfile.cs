using AutoMapper;
using ParkingReservation.Dtos;
using ParkingReservation.Models;

namespace ParkingReservation.Mapping
{
    public class SpaceProfile : Profile
    {
        public SpaceProfile()
        {
            CreateMap<Space, SpaceDto>();
            CreateMap<Space, SpaceDetailDto>();
        }

    }
}
