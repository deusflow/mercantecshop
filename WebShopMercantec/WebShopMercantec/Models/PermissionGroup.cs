using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class PermissionGroup
{
    public uint Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Permissions { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }
}
