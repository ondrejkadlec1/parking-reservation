namespace ParkingReservation.Models.Interfaces
{
    public interface IOccupation
    {
        public int SpaceNumber { get; set; }

        public DateTime BeginsAt { get; set; }

        public DateTime EndsAt { get; set; }
    }
}
