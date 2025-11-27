using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class Location
{
    public uint Id { get; set; }

    public string? Name { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Country { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public string? Address { get; set; }

    public string? Address2 { get; set; }

    public string? Zip { get; set; }

    public string? Fax { get; set; }

    public string? Phone { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? ParentId { get; set; }

    public string? Currency { get; set; }

    public string? LdapOu { get; set; }

    public int? ManagerId { get; set; }

    public string? Image { get; set; }
}
