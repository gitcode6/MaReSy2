using System;
using System.Collections.Generic;

namespace MaReSy2_Api.Models;

public partial class ProductsSet
{
    public int ProductSetId { get; set; }

    public int SetId { get; set; }

    public int ProductId { get; set; }

    public int Productamount { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Set Set { get; set; } = null!;
}
