using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class OauthPersonalAccessClient
{
    public ulong Id { get; set; }

    public ulong ClientId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
