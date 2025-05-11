namespace ParkingReservation.Dtos.Spaces
{
    public record SpaceDto
    {
        public int SpaceNumber { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
