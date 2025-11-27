using System.ComponentModel.DataAnnotations;

namespace WebShopMercantec.DTOs.Common;

/// <summary>
/// Простое представление денежного значения в единой валюте.
/// </summary>
public sealed record MoneyDto
{
    [Range(0, double.MaxValue)]
    public decimal Amount { get; init; }

    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; init; } = "DKK";
}

