using System.ComponentModel.DataAnnotations;
using WebShopMercantec.DTOs.Common;

namespace WebShopMercantec.DTOs.Cart;

/// <summary>
/// Финансовые итоги корзины.
/// </summary>
public sealed record CartTotalsDto
{
    [Required]
    public MoneyDto Subtotal { get; init; } = new();

    [Required]
    public MoneyDto Tax { get; init; } = new();

    [Required]
    public MoneyDto Shipping { get; init; } = new();

    [Required]
    public MoneyDto Discount { get; init; } = new();

    [Required]
    public MoneyDto GrandTotal { get; init; } = new();
}
