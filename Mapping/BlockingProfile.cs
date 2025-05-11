using AutoMapper;
using ParkingReservation.Dtos.Blockings;
using ParkingReservation.Models;

namespace ParkingReservation.Mapping
{
    public class BlockingProfile : Profile
    {
        public BlockingProfile()
        {
            CreateMap<Blocking, BlockingDto>();
            CreateMap<CreateBlockingDto, Blocking>();
        }

    }
}
