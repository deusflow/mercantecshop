using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class RequestedAsset
{
    public uint Id { get; set; }

    public int AssetId { get; set; }

    public int UserId { get; set; }

    public DateTime? AcceptedAt { get; set; }

    public DateTime? DeniedAt { get; set; }

    public string Notes { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
