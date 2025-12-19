using WebShopMercantec.Models;
using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Mapping;

/// <summary>
/// Маппинг для Category -> CategoryDto
/// Централизованное место для преобразования Entity в DTO
/// </summary>
public static class CategoryMapping
{
    /// <summary>
    /// Преобразовать Category в CategoryDto
    /// </summary>
    public static CategoryDto MapToDto(Category category, int itemsCount = 0)
    {
        return new CategoryDto
        {
            Id = (int)category.Id,
            Name = category.Name,
            CategoryType = category.CategoryType,
            ItemsCount = itemsCount,
            Image = category.Image,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }

    /// <summary>
    /// Преобразовать список Category в список CategoryDto
    /// </summary>
    public static IEnumerable<CategoryDto> MapToDtos(IEnumerable<Category> categories, 
        Func<uint, int>? getItemsCount = null)
    {
        return categories.Select(c => MapToDto(c, getItemsCount?.Invoke(c.Id) ?? 0));
    }
}

