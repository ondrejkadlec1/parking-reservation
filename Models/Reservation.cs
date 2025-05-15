using System;
using System.Collections.Generic;

namespace ParkingReservation.Models;

public partial class Reservation
{
    public Guid Id { get; set; }

    public int StateId { get; set; }

    public int SpaceNumber { get; set; }

    public int TypeId { get; set; }

    public DateTime BeginsAt { get; set; }

    public DateTime EndsAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid UserId { get; set; }

    public string? Comment { get; set; }

    public virtual Space SpaceNumberNavigation { get; set; } = null!;

    public virtual State State { get; set; } = null!;

    public virtual ReservationType Type { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
