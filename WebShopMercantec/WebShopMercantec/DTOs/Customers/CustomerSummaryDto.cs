using System.ComponentModel.DataAnnotations;

namespace WebShopMercantec.DTOs.Customers;

/// <summary>
/// Сжатая информация о клиенте.
/// </summary>
public sealed record CustomerSummaryDto
{
    [Required]
    public Guid Id { get; init; }

    [Required]
    [StringLength(64)]
    public string FullName { get; init; } = string.Empty;

    [EmailAddress]
    [StringLength(128)]
    public string? Email { get; init; }
}

