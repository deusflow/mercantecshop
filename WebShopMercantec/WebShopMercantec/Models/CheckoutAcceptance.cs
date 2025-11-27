using System;
using System.Collections.Generic;

namespace WebShopMercantec.Models;

public partial class CheckoutAcceptance
{
    public uint Id { get; set; }

    public string CheckoutableType { get; set; } = null!;

    public ulong CheckoutableId { get; set; }

    public int? AssignedToId { get; set; }

    public string? SignatureFilename { get; set; }

    public string? Note { get; set; }

    public DateTime? AcceptedAt { get; set; }

    public DateTime? DeclinedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? StoredEula { get; set; }

    public string? StoredEulaFile { get; set; }
}
