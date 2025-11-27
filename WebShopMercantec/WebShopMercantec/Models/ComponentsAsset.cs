using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class ComponentsAsset
{
    public uint Id { get; set; }

    public int? CreatedBy { get; set; }

    public int? AssignedQty { get; set; }

    public int? ComponentId { get; set; }

    public int? AssetId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Note { get; set; }
}
