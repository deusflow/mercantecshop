using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class Consumable
{
    public uint Id { get; set; }

    public string? Name { get; set; }

    public int? CategoryId { get; set; }

    public int? LocationId { get; set; }

    public int? CreatedBy { get; set; }

    public int? SupplierId { get; set; }

    public int Qty { get; set; }

    public bool Requestable { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    public decimal? PurchaseCost { get; set; }

    public string? OrderNumber { get; set; }

    public uint? CompanyId { get; set; }

    public int? MinAmt { get; set; }

    public string? ModelNumber { get; set; }

    public int? ManufacturerId { get; set; }

    public string? ItemNo { get; set; }

    public string? Image { get; set; }

    public string? Notes { get; set; }
}
