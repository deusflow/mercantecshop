using System.ComponentModel.DataAnnotations;

namespace WebShopMercantec.DTOs.Catalog;

/// <summary>
/// Краткое описание категории для вложения в другие DTO.
/// </summary>
public sealed record CategorySummaryDto
{
    [Required]
    public Guid Id { get; init; }

    [Required]
    [StringLength(64)]
    public string Name { get; init; } = string.Empty;

    [StringLength(128)]
    public string? Slug { get; init; }
}

