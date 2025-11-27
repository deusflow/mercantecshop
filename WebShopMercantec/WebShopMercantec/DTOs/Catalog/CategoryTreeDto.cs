using System.ComponentModel.DataAnnotations;

namespace WebShopMercantec.DTOs.Catalog;

/// <summary>
/// Полное описание категории с дочерними элементами.
/// </summary>
public sealed record CategoryTreeDto
{
    [Required]
    public Guid Id { get; init; }

    [Required]
    [StringLength(64)]
    public string Name { get; init; } = string.Empty;

    [StringLength(128)]
    public string? Slug { get; init; }

    public Guid? ParentId { get; init; }

    public IReadOnlyCollection<CategoryTreeDto> Children { get; init; } = Array.Empty<CategoryTreeDto>();
}

