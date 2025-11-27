using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class AssetMaintenance
{
    public uint Id { get; set; }

    public uint AssetId { get; set; }

    public uint SupplierId { get; set; }

    public string AssetMaintenanceType { get; set; } = null!;

    public string Title { get; set; } = null!;

    public bool IsWarranty { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? CompletionDate { get; set; }

    public int? AssetMaintenanceTime { get; set; }

    public string? Notes { get; set; }

    public decimal? Cost { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }
}
