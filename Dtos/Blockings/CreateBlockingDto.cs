using ParkingReservation.Attributes;

namespace ParkingReservation.Dtos.Blockings
{
    [MinimalDuration(30)]
    public record CreateBlockingDto
    {
        [MinutesResolution(30)]
        public DateTime BeginsAt { get; set; }
        [MinutesResolution(30)]
        public DateTime EndsAt { get; set; }

        public int SpaceNumber { get; set; }
    }
}
