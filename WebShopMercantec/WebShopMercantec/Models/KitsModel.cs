using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class KitsModel
{
    public uint Id { get; set; }

    public int? KitId { get; set; }

    public int? ModelId { get; set; }

    public int Quantity { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public ulong? CreatedBy { get; set; }
}
