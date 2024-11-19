using System;
using System.Collections.Generic;

namespace MaReSy2_Api.Models;

public partial class SingleProduct
{
    public int SingleProductId { get; set; }

    public string SingleProductName { get; set; } = null!;

    public string SingleProductNumber { get; set; } = null!;

    public bool SingleProductActive { get; set; }

    public int ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}
