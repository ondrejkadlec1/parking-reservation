namespace ParkingReservation.Dtos
{
    public record ReservationRequestDto
    {
        public DateTime BeginsAt { get; set; }

        public DateTime EndsAt { get; set; }

    }
}
