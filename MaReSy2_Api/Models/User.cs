using System;
using System.Collections.Generic;

namespace MaReSy2_Api.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int RoleId { get; set; }

    public virtual ICollection<Rental> RentalRentalAblehnungUserNavigations { get; set; } = new List<Rental>();

    public virtual ICollection<Rental> RentalRentalAuslieferungUserNavigations { get; set; } = new List<Rental>();

    public virtual ICollection<Rental> RentalRentalFreigabeUserNavigations { get; set; } = new List<Rental>();

    public virtual ICollection<Rental> RentalRentalZurückgabeUserNavigations { get; set; } = new List<Rental>();

    public virtual ICollection<Rental> RentalUsers { get; set; } = new List<Rental>();

    public virtual Role Role { get; set; } = null!;
}
