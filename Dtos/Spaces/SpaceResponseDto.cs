namespace ParkingReservation.Dtos.Spaces
{
    public record SpaceResponseDto
    {
        public int SpaceNumber { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
