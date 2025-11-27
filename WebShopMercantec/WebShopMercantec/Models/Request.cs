using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class Request
{
    public uint Id { get; set; }

    public int AssetId { get; set; }

    public int? UserId { get; set; }

    public string RequestCode { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}
