using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class LicenseSeat
{
    public uint Id { get; set; }

    public int? LicenseId { get; set; }

    public int? AssignedTo { get; set; }

    public string? Notes { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? AssetId { get; set; }
}
