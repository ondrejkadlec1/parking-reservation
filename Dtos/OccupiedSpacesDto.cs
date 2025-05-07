namespace ParkingReservation.Dtos
{
    public record OccupiedSpacesDto
    {
        public int OccupiedCount { get; set; }
        public int TotalCount { get; set; }
        public bool Avialible { get; set; }
    }
}
