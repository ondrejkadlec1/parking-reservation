﻿using ParkingReservation.Attributes;
using ParkingReservation.Dtos.Interfaces;


namespace ParkingReservation.Dtos.Reservations
{
    public record ReservationRequestDto : ITimeInterval
    {
        [MinutesInAdvance(30)]
        public DateTime BeginsAt { get; set; }
        public DateTime EndsAt { get; set; }

    }
}
