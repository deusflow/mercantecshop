using System.ComponentModel.DataAnnotations;
using WebShopMercantec.DTOs.Common;

namespace WebShopMercantec.DTOs.Customers;

/// <summary>
/// Полное представление клиента для CRM и заказов.
/// </summary>
public sealed record CustomerDto
{
    [Required]
    public Guid Id { get; init; }

    [Required]
    [StringLength(64)]
    public string FirstName { get; init; } = string.Empty;

    [Required]
    [StringLength(64)]
    public string LastName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(128)]
    public string Email { get; init; } = string.Empty;

    [Phone]
    [StringLength(32)]
    public string? Phone { get; init; }

    [Required]
    public AddressDto DefaultShippingAddress { get; init; } = new();

    public AddressDto? DefaultBillingAddress { get; init; }

    public bool IsActive { get; init; } = true;

    public DateTimeOffset CreatedAt { get; init; }
}

