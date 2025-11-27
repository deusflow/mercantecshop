using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class KitsConsumable
{
    public uint Id { get; set; }

    public int? KitId { get; set; }

    public int? ConsumableId { get; set; }

    public int Quantity { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public ulong? CreatedBy { get; set; }
}
