using System.ComponentModel.DataAnnotations;
using WebShopMercantec.DTOs.Cart;
using WebShopMercantec.DTOs.Common;
using WebShopMercantec.DTOs.Customers;

namespace WebShopMercantec.DTOs.Orders;

/// <summary>
/// Представление заказа для API и UI.
/// </summary>
public sealed record OrderDto
{
    [Required]
    public Guid Id { get; init; }

    [Required]
    public string OrderNumber { get; init; } = string.Empty;

    [Required]
    public OrderStatus Status { get; init; }

    [Required]
    public CustomerSummaryDto Customer { get; init; } = new();

    [Required]
    public IReadOnlyCollection<OrderItemDto> Items { get; init; } = Array.Empty<OrderItemDto>();

    [Required]
    public CartTotalsDto Totals { get; init; } = new();

    [Required]
    public AddressDto ShippingAddress { get; init; } = new();

    [Required]
    public AddressDto BillingAddress { get; init; } = new();

    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? PaidAt { get; init; }

    public DateTimeOffset? ShippedAt { get; init; }
}

