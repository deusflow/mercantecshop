using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class ModelsCustomField
{
    public uint Id { get; set; }

    public int AssetModelId { get; set; }

    public int CustomFieldId { get; set; }

    public string? DefaultValue { get; set; }
}
