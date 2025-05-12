using ParkingReservation.Attributes;
using ParkingReservation.Dtos.Interfaces;

namespace ParkingReservation.Dtos.Reservations
{
    public record CreateBlockingDto : ITimeInterval
    {
        public DateTime BeginsAt { get; set; }
        [MinutesInAdvance(0)]
        public DateTime EndsAt { get; set; }

        public int SpaceNumber { get; set; }
        public string Comment { get; set; }
    }
}
