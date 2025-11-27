using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class Category
{
    public uint Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? EulaText { get; set; }

    public bool UseDefaultEula { get; set; }

    public bool RequireAcceptance { get; set; }

    public string? CategoryType { get; set; }

    public bool CheckinEmail { get; set; }

    public string? Image { get; set; }
}
