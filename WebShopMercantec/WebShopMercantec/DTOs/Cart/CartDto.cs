using System.ComponentModel.DataAnnotations;

namespace WebShopMercantec.DTOs.Cart;

/// <summary>
/// Снимок корзины пользователя.
/// </summary>
public sealed record CartDto
{
    [Required]
    public Guid CartId { get; init; }

    [Required]
    public Guid CustomerId { get; init; }

    public IReadOnlyCollection<CartItemDto> Items { get; init; } = Array.Empty<CartItemDto>();

    [Required]
    public CartTotalsDto Totals { get; init; } = new();

    public bool IsLocked { get; init; }

    public DateTimeOffset UpdatedAt { get; init; }
}

