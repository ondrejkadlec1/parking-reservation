namespace ParkingReservation.Dtos.Interfaces
{
    public interface IOccupationDto
    {
        public int SpaceNumber { get; set; }

        public DateTime BeginsAt { get; set; }

        public DateTime EndsAt { get; set; }
    }
}
