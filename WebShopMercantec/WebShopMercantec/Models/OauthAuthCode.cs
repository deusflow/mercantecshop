using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class OauthAuthCode
{
    public string Id { get; set; } = null!;

    public ulong UserId { get; set; }

    public ulong ClientId { get; set; }

    public string? Scopes { get; set; }

    public bool Revoked { get; set; }

    public DateTime? ExpiresAt { get; set; }
}
