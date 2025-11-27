using System.ComponentModel.DataAnnotations;

namespace WebShopMercantec.DTOs.Products;

/// <summary>
/// Мини-DTO для медиа-данных товара (картинка, превью).
/// </summary>
public sealed record ProductMediaSnapshotDto
{
    [Url]
    public string? ThumbnailUrl { get; init; }

    [Url]
    public string? FullImageUrl { get; init; }

    [StringLength(128)]
    public string? AltText { get; init; }
}

