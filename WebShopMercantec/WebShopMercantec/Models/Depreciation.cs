using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class Depreciation
{
    public uint Id { get; set; }

    public string Name { get; set; } = null!;

    public int Months { get; set; }

    public decimal? DepreciationMin { get; set; }

    public string DepreciationType { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }
}
