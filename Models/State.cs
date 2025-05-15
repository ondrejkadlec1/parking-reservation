using System;
using System.Collections.Generic;

namespace ParkingReservation.Models;

public partial class State
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string NameCs { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
