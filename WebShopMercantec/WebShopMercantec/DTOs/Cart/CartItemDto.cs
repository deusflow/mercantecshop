using System.ComponentModel.DataAnnotations;
using WebShopMercantec.DTOs.Common;
using WebShopMercantec.DTOs.Products;

namespace WebShopMercantec.DTOs.Cart;

/// <summary>
/// Позиция в корзине.
/// </summary>
public sealed record CartItemDto
{
    [Required]
    public Guid ProductId { get; init; }

    [Required]
    public string ProductName { get; init; } = string.Empty;

    [Required]
    public MoneyDto UnitPrice { get; init; } = new();

    [Required]
    public MoneyDto LineTotal { get; init; } = new();

    [Range(1, 99)]
    public int Quantity { get; init; }

    public ProductMediaSnapshotDto? Media { get; init; }
}

