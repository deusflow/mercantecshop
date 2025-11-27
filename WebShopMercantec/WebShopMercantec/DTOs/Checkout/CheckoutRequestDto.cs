using System.ComponentModel.DataAnnotations;
using WebShopMercantec.DTOs.Cart;
using WebShopMercantec.DTOs.Common;
using WebShopMercantec.DTOs.Customers;

namespace WebShopMercantec.DTOs.Checkout;

/// <summary>
/// Запрос на оформление заказа.
/// </summary>
public sealed record CheckoutRequestDto
{
    [Required]
    public Guid CartId { get; init; }

    [Required]
    public CustomerDto Customer { get; init; } = new();

    [Required]
    public AddressDto ShippingAddress { get; init; } = new();

    [Required]
    public AddressDto BillingAddress { get; init; } = new();

    [Required]
    public PaymentDetailsDto Payment { get; init; } = new();

    [Required]
    public CartTotalsDto TotalsSnapshot { get; init; } = new();

    [StringLength(512)]
    public string? CustomerNote { get; init; }
}

