using System.ComponentModel.DataAnnotations;

namespace WebShopMercantec.DTOs.Products;

/// <summary>
/// Состояние складских остатков по товару.
/// </summary>
public sealed record ProductInventoryDto
{
    [Required]
    public Guid ProductId { get; init; }

    [Range(0, int.MaxValue)]
    public int Available { get; init; }

    [Range(0, int.MaxValue)]
    public int Reserved { get; init; }

    [Range(0, int.MaxValue)]
    public int Incoming { get; init; }

    public bool AllowBackorder { get; init; }
}

