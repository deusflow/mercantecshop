using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class License
{
    public uint Id { get; set; }

    public string? Name { get; set; }

    public string? Serial { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    public decimal? PurchaseCost { get; set; }

    public string? OrderNumber { get; set; }

    public int Seats { get; set; }

    public string? Notes { get; set; }

    public int? CreatedBy { get; set; }

    public int? DepreciationId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? LicenseName { get; set; }

    public string? LicenseEmail { get; set; }

    public bool? Depreciate { get; set; }

    public int? SupplierId { get; set; }

    public DateOnly? ExpirationDate { get; set; }

    public string? PurchaseOrder { get; set; }

    public DateOnly? TerminationDate { get; set; }

    public bool? Maintained { get; set; }

    public bool? Reassignable { get; set; }

    public uint? CompanyId { get; set; }

    public int? ManufacturerId { get; set; }

    public int? CategoryId { get; set; }

    public int? MinAmt { get; set; }
}
