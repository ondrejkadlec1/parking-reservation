using System;
using System.Collections.Generic;

namespace ParkingReservation.Models;

public partial class Space
{
    public int SpaceNumber { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Blocking> Blockings { get; set; } = new List<Blocking>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
