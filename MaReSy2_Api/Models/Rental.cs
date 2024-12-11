using System;
using System.Collections.Generic;

namespace MaReSy2_Api.Models;

public partial class Rental
{
    public int RentalId { get; set; }

    public int UserId { get; set; }

    public int? SetId { get; set; }

    public DateTime RentalStart { get; set; }

    public DateTime RentalEnd { get; set; }

    public DateTime RentalAnforderung { get; set; }

    public DateTime? RentalFreigabe { get; set; }

    public int? RentalFreigabeUser { get; set; }

    public DateTime? RentalAblehnung { get; set; }

    public int? RentalAblehnungUser { get; set; }

    public DateTime? RentalAuslieferung { get; set; }

    public int? RentalAuslieferungUser { get; set; }

    public DateTime? RentalZurückgabe { get; set; }

    public int? RentalZurückgabeUser { get; set; }

    public DateTime? RentalStornierung { get; set; }

    public int Status { get; set; }

    public string? RentalNote { get; set; }

    public virtual User? RentalAblehnungUserNavigation { get; set; }

    public virtual User? RentalAuslieferungUserNavigation { get; set; }

    public virtual User? RentalFreigabeUserNavigation { get; set; }

    public virtual User? RentalZurückgabeUserNavigation { get; set; }

    public virtual Set? Set { get; set; }

    public virtual Status StatusNavigation { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual ICollection<SingleProduct> SingleProducts { get; set; } = new List<SingleProduct>();
}
