namespace ParkingReservation.Dtos
{
    public record CreateBlockingDto
    {
        public DateTime BeginsAt { get; set; }

        public DateTime EndsAt { get; set; }

        public int SpaceNumber { get; set; }
    }
}
