using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class Throttle
{
    public uint Id { get; set; }

    public uint? UserId { get; set; }

    public string? IpAddress { get; set; }

    public int Attempts { get; set; }

    public bool Suspended { get; set; }

    public bool Banned { get; set; }

    public DateTime? LastAttemptAt { get; set; }

    public DateTime? SuspendedAt { get; set; }

    public DateTime? BannedAt { get; set; }
}
