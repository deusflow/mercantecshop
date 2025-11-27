using System.ComponentModel.DataAnnotations;

namespace WebShopMercantec.DTOs.Common;

/// <summary>
/// Унифицированное представление почтового адреса.
/// </summary>
public sealed record AddressDto
{
    [Required]
    [StringLength(128)]
    public string Line1 { get; init; } = string.Empty;

    [StringLength(128)]
    public string? Line2 { get; init; }

    [Required]
    [StringLength(64)]
    public string City { get; init; } = string.Empty;

    [StringLength(64)]
    public string? State { get; init; }

    [Required]
    [StringLength(16, MinimumLength = 3)]
    public string PostalCode { get; init; } = string.Empty;

    [Required]
    [StringLength(64)]
    public string Country { get; init; } = "Denmark";
}

