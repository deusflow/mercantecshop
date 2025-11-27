using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class AssetUpload
{
    public uint Id { get; set; }

    public int? UserId { get; set; }

    public string Filename { get; set; } = null!;

    public int AssetId { get; set; }

    public string? Filenotes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}
