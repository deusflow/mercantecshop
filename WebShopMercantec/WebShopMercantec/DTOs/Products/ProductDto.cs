using System.ComponentModel.DataAnnotations;
using WebShopMercantec.DTOs.Catalog;
using WebShopMercantec.DTOs.Common;

namespace WebShopMercantec.DTOs.Products;

/// <summary>
/// Представление товара для витрины и API.
/// </summary>
public sealed record ProductDto
{
    [Required]
    public Guid Id { get; init; }

    [Required]
    [StringLength(128)]
    public string Name { get; init; } = string.Empty;

    [StringLength(512)]
    public string? Description { get; init; }

    [Required]
    public MoneyDto Price { get; init; } = new();

    public MoneyDto? ListPrice { get; init; }

    [Range(0, 100)]
    public decimal? DiscountPercent { get; init; }

    [Required]
    public CategorySummaryDto Category { get; init; } = new();

    [Url]
    public string? ImageUrl { get; init; }

    [Range(0, int.MaxValue)]
    public int StockAvailable { get; init; }

    public bool IsPublished { get; init; }
}

