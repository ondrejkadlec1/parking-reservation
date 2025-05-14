namespace ParkingReservation.Dtos.Spaces
{
    public record Availability
    {
        public int OccupiedCount { get; set; }
        public int TotalCount { get; set; }
        public bool Available { get; set; }
    }
}
