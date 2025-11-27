using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class AssetLog
{
    public uint Id { get; set; }

    public int? UserId { get; set; }

    public string ActionType { get; set; } = null!;

    public int AssetId { get; set; }

    public int? CheckedoutTo { get; set; }

    public int? LocationId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? AssetType { get; set; }

    public string? Note { get; set; }

    public string? Filename { get; set; }

    public DateTime? RequestedAt { get; set; }

    public DateTime? AcceptedAt { get; set; }

    public int? AccessoryId { get; set; }

    public int? AcceptedId { get; set; }

    public int? ConsumableId { get; set; }

    public DateOnly? ExpectedCheckin { get; set; }

    public int? ComponentId { get; set; }
}
