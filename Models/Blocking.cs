using System;
using System.Collections.Generic;

namespace ParkingReservation.Models;

public partial class Blocking
{
    public Guid Id { get; set; }

    public int SpaceNumber { get; set; }

    public DateTime BeginsAt { get; set; }

    public DateTime EndsAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public string AdminId { get; set; } = null!;

    public virtual Space SpaceNumberNavigation { get; set; } = null!;
}
