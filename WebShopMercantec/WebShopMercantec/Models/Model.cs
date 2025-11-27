using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class Model
{
    public uint Id { get; set; }

    public string Name { get; set; } = null!;

    public string? ModelNumber { get; set; }

    public int? MinAmt { get; set; }

    public int? ManufacturerId { get; set; }

    public int? CategoryId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? DepreciationId { get; set; }

    public int? CreatedBy { get; set; }

    public int? Eol { get; set; }

    public string? Image { get; set; }

    public bool DeprecatedMacAddress { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? FieldsetId { get; set; }

    public string? Notes { get; set; }

    public sbyte Requestable { get; set; }
}
