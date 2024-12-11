using System;
using System.Collections.Generic;

namespace MaReSy2_Api.Models;

public partial class Set
{
    public int SetId { get; set; }

    public string Setname { get; set; } = null!;

    public string? Setdescription { get; set; }

    public byte[]? Setimage { get; set; }

    public bool Setactive { get; set; }

    public virtual ICollection<ProductsSet> ProductsSets { get; set; } = new List<ProductsSet>();

    public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}
