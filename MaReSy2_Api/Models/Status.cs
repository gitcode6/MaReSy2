using System;
using System.Collections.Generic;

namespace MaReSy2_Api.Models;

public partial class Status
{
    public int Id { get; set; }

    public string Bezeichnung { get; set; } = null!;

    public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}
