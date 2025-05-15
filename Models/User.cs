using System;
using System.Collections.Generic;

namespace ParkingReservation.Models;

public partial class User
{
    public Guid Id { get; set; }

    public DateTime AddedAt { get; set; }

    public bool IsActive { get; set; }

    public bool IsAdmin { get; set; }

    public string DisplayName { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual ICollection<Space> Spaces { get; set; } = new List<Space>();
}
