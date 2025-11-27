using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class CustomField
{
    public uint Id { get; set; }

    public string Name { get; set; } = null!;

    public string Format { get; set; } = null!;

    public string Element { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public string? FieldValues { get; set; }

    public bool FieldEncrypted { get; set; }

    public string? DbColumn { get; set; }

    public string? HelpText { get; set; }

    public bool ShowInEmail { get; set; }

    public bool? ShowInRequestableList { get; set; }

    public bool? IsUnique { get; set; }

    public bool? DisplayInUserView { get; set; }

    public bool? AutoAddToFieldsets { get; set; }

    public bool? ShowInListview { get; set; }
}
