using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class Department
{
    public uint Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Fax { get; set; }

    public string? Phone { get; set; }

    public int CreatedBy { get; set; }

    public int? CompanyId { get; set; }

    public int? LocationId { get; set; }

    public int? ManagerId { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? Image { get; set; }
}
