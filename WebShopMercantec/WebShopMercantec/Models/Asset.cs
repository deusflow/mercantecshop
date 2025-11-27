using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class Asset
{
    public uint Id { get; set; }

    public string? Name { get; set; }

    public string? AssetTag { get; set; }

    public int? ModelId { get; set; }

    public string? Serial { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    public DateOnly? AssetEolDate { get; set; }

    public bool EolExplicit { get; set; }

    public decimal? PurchaseCost { get; set; }

    public string? OrderNumber { get; set; }

    public int? AssignedTo { get; set; }

    public string? Notes { get; set; }

    public string? Image { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? Physical { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? StatusId { get; set; }

    public bool? Archived { get; set; }

    public int? WarrantyMonths { get; set; }

    public bool? Depreciate { get; set; }

    public int? SupplierId { get; set; }

    public sbyte Requestable { get; set; }

    public int? RtdLocationId { get; set; }

    public string? SnipeitMacAddress1 { get; set; }

    public string? Accepted { get; set; }

    public DateTime? LastCheckout { get; set; }

    public DateTime? LastCheckin { get; set; }

    public DateOnly? ExpectedCheckin { get; set; }

    public uint? CompanyId { get; set; }

    public string? AssignedType { get; set; }

    public DateTime? LastAuditDate { get; set; }

    public DateOnly? NextAuditDate { get; set; }

    public int? LocationId { get; set; }

    public int CheckinCounter { get; set; }

    public int CheckoutCounter { get; set; }

    public int RequestsCounter { get; set; }

    public bool? Byod { get; set; }
}
