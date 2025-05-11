using ParkingReservation.Attributes;


namespace ParkingReservation.Dtos.Reservations
{
    [MinimalDuration(30)]
    public record ReservationRequestDto
    {
        [TimeInAdvance(30)]
        [MinutesResolution(30)]
        public DateTime BeginsAt { get; set; }
        [MinutesResolution(30)]
        public DateTime EndsAt { get; set; }

    }
}
