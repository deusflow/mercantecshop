using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class ActionLog
{
    public uint Id { get; set; }

    public int? CreatedBy { get; set; }

    public string ActionType { get; set; } = null!;

    public int? TargetId { get; set; }

    public string? TargetType { get; set; }

    public int? LocationId { get; set; }

    public string? Note { get; set; }

    public string? Filename { get; set; }

    public string ItemType { get; set; } = null!;

    public int ItemId { get; set; }

    public DateOnly? ExpectedCheckin { get; set; }

    public int? AcceptedId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? ThreadId { get; set; }

    public int? CompanyId { get; set; }

    public string? AcceptSignature { get; set; }

    public string? LogMeta { get; set; }

    public DateTime? ActionDate { get; set; }

    public string? StoredEula { get; set; }

    public string? ActionSource { get; set; }

    public string? RemoteIp { get; set; }

    public string? UserAgent { get; set; }
}
