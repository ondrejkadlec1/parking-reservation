using AutoMapper;
using ParkingReservation.Dtos.Spaces;
using ParkingReservation.Models;

namespace ParkingReservation.Mapping
{
    public class SpaceProfile : Profile
    {
        public SpaceProfile()
        {
            CreateMap<Space, SpaceResponseDto>();
        }

    }
}
