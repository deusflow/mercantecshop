using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class CheckoutRequest
{
    public uint Id { get; set; }

    public int UserId { get; set; }

    public int RequestableId { get; set; }

    public string RequestableType { get; set; } = null!;

    public int Quantity { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? CanceledAt { get; set; }

    public DateTime? FulfilledAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}
