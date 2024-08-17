using System;
using System.Collections.Generic;

namespace MaReSy2_Api.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string Productname { get; set; } = null!;

    public string? Productdescription { get; set; }

    public byte[]? Productimage { get; set; }

    public int Productactive { get; set; }

    public int Productamount { get; set; }

    public virtual ICollection<ProductsSet> ProductsSets { get; set; } = new List<ProductsSet>();

    public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}
