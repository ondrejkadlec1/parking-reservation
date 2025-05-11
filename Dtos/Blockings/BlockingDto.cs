namespace ParkingReservation.Dtos.Blockings
{
    public record BlockingDto
    {
        public Guid Id { get; set; }

        public DateTime BeginsAt { get; set; }

        public DateTime EndsAt { get; set; }

        public string AdminId { get; set; } = null!;

        public int SpaceNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
