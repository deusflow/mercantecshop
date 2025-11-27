using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class ReportTemplate
{
    public ulong Id { get; set; }

    public int? CreatedBy { get; set; }

    public string Name { get; set; } = null!;

    public string Options { get; set; } = null!;

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
