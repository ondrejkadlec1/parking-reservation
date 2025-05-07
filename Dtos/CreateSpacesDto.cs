namespace ParkingReservation.Dtos
{
    public record CreateSpacesDto
    {
        public int Count { get; set; }
        public DateTime AvialibleFrom { get; set; }
    }
}
