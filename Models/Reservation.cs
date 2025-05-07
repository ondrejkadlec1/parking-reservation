using System;
using System.Collections.Generic;

namespace ParkingReservation.Models;

public partial class Reservation
{
    public Guid Id { get; set; }

    public int StateId { get; set; }

    public int SpaceNumber { get; set; }

    public DateTime BeginsAt { get; set; }

    public DateTime EndsAt { get; set; }

    public DateTime IssuedAt { get; set; }

    public string UserId { get; set; } = null!;

    public virtual Space SpaceNumberNavigation { get; set; } = null!;

    public virtual State State { get; set; } = null!;
}
