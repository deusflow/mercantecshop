using System.ComponentModel.DataAnnotations;

namespace WebShopMercantec.DTOs.Checkout;

/// <summary>
/// Данные, необходимые для выбранного метода оплаты.
/// </summary>
public sealed record PaymentDetailsDto
{
    [Required]
    public PaymentMethodType Method { get; init; }

    [StringLength(4, MinimumLength = 4)]
    public string? Last4Digits { get; init; }

    [StringLength(64)]
    public string? ProviderReference { get; init; }

    public bool IsTokenized { get; init; }
}

