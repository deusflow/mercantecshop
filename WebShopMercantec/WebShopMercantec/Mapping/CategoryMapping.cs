using WebShopMercantec.Models;
using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Mapping;

public static class CategoryMapping
{
    
    public static CategoryDto MapToDto(Category category, int itemsCount = 0)
    {
        return new CategoryDto
        {
            Id = (int)category.Id,
            Name = category.Name,
            CategoryType = category.CategoryType,
            ShowInCatalog = category.CheckinEmail,
            ItemsCount = itemsCount,
            Image = category.Image,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }

    
    public static IEnumerable<CategoryDto> MapToDtos(IEnumerable<Category> categories, 
        Func<uint, int>? getItemsCount = null)
    {
        return categories.Select(c => MapToDto(c, getItemsCount?.Invoke(c.Id) ?? 0));
    }
}
