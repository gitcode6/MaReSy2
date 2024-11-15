using System;
using System.Collections.Generic;

namespace MaReSy2_Api.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string Productname { get; set; } = null!;

    public string? Productdescription { get; set; }

    public byte[]? Productimage { get; set; }

    public bool ProductActive { get; set; }

    public virtual ICollection<ProductsSet> ProductsSets { get; set; } = new List<ProductsSet>();

    public virtual ICollection<SingleProduct> SingleProducts { get; set; } = new List<SingleProduct>();
}
