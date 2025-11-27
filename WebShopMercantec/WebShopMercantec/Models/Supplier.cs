using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class Supplier
{
    public uint Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public string? Address2 { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Country { get; set; }

    public string? Phone { get; set; }

    public string? Fax { get; set; }

    public string? Email { get; set; }

    public string? Contact { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? Zip { get; set; }

    public string? Url { get; set; }

    public string? Image { get; set; }
}
