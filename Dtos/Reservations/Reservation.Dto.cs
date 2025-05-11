namespace ParkingReservation.Dtos.Reservations
{
    public record ReservationDto
    {
        public Guid Id { get; set; }
        public DateTime BeginsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public DateTime IssuedAt { get; set; }
        public string UserId { get; set; } = null!;
        public string State { get; set; } = null!;
        public int SpaceNumber { get; set; }
    }
}
