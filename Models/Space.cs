using System;
using System.Collections.Generic;

namespace ParkingReservation.Models;

public partial class Space
{
    public int SpaceNumber { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
