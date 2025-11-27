using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class StatusLabel
{
    public uint Id { get; set; }

    public string? Name { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public bool Deployable { get; set; }

    public bool Pending { get; set; }

    public bool Archived { get; set; }

    public string? Notes { get; set; }

    public string? Color { get; set; }

    public bool? ShowInNav { get; set; }

    public bool? DefaultLabel { get; set; }
}
