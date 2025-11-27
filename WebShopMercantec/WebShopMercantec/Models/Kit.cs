using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class Kit
{
    public uint Id { get; set; }

    public string? Name { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public ulong? CreatedBy { get; set; }
}
