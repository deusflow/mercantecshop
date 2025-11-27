using System.ComponentModel.DataAnnotations;
using WebShopMercantec.DTOs.Checkout;
using WebShopMercantec.DTOs.Common;

namespace WebShopMercantec.DTOs.Orders;

/// <summary>
/// Результат обработки платежа.
/// </summary>
public sealed record PaymentReceiptDto
{
    [Required]
    public Guid OrderId { get; init; }

    [Required]
    public PaymentMethodType Method { get; init; }

    [Required]
    public MoneyDto CapturedAmount { get; init; } = new();

    [StringLength(64)]
    public string TransactionReference { get; init; } = string.Empty;

    public DateTimeOffset CapturedAt { get; init; }
}

