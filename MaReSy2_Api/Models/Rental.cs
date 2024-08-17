using System;
using System.Collections.Generic;

namespace MaReSy2_Api.Models;

public partial class Rental
{
    public int RentalId { get; set; }

    public string Username { get; set; } = null!;

    public int? SetId { get; set; }

    public int? ProductId { get; set; }

    public int RentalAmount { get; set; }

    public DateTime RentalCreated { get; set; }

    public DateTime RentalStart { get; set; }

    public DateTime RentalEnd { get; set; }

    public DateTime? RentalDelivery { get; set; }

    public DateTime? RentalReturned { get; set; }

    public DateTime? RentalCanceled { get; set; }

    public DateTime? RentalFree { get; set; }

    public string? RentalCanceledUserName { get; set; }

    public string? RentalNote { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Set? Set { get; set; }

    public virtual User UsernameNavigation { get; set; } = null!;
}
