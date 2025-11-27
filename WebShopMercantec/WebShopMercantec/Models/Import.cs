using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class Import
{
    public uint Id { get; set; }

    public string? Name { get; set; }

    public string FilePath { get; set; } = null!;

    public int Filesize { get; set; }

    public string? ImportType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? HeaderRow { get; set; }

    public string? FirstRow { get; set; }

    public string? FieldMap { get; set; }

    public ulong? CreatedBy { get; set; }
}
