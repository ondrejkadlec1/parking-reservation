using ParkingReservation.Attributes;

namespace ParkingReservation.Dtos.Interfaces
{
    [MinimalDuration(30)]
    public interface ITimeInterval
    {
        [MinutesResolution(30)]
        public DateTime BeginsAt { get; set; }
        [MinutesResolution(30)]
        public DateTime EndsAt { get; set; }
    }
}
