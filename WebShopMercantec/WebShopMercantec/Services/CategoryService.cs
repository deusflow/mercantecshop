using WebShopMercantec.Models;
using WebShopMercantec.Shared.DTOs;
using WebShopMercantec.Repositories;
using WebShopMercantec.Exceptions;
using WebShopMercantec.Mapping;

namespace WebShopMercantec.Services;

/// <summary>
/// Сервис для работы с категориями
/// Использует Repository Pattern и обрабатывает бизнес-логику
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(IUnitOfWork unitOfWork, ILogger<CategoryService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Получить все активные категории с количеством элементов
    /// </summary>
    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        _logger.LogInformation("Getting all categories");
        
        var categories = await _unitOfWork.Categories.GetAllActiveCategoriesAsync();
        
        var categoryDtos = new List<CategoryDto>();
        foreach (var category in categories)
        {
            var itemsCount = await _unitOfWork.Categories.GetItemsCountAsync(category.Id);
            categoryDtos.Add(CategoryMapping.MapToDto(category, itemsCount));
        }
        
        _logger.LogInformation("Found {Count} categories", categoryDtos.Count);
        return categoryDtos;
    }

    /// <summary>
    /// Получить категорию по ID
    /// </summary>
    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        _logger.LogInformation("Getting category with ID: {CategoryId}", id);
        
        var category = await _unitOfWork.Categories.GetActiveCategoryByIdAsync((uint)id);
        
        if (category == null)
        {
            _logger.LogWarning("Category not found: {CategoryId}", id);
            throw new NotFoundException("Category", id);
        }
        
        var itemsCount = await _unitOfWork.Categories.GetItemsCountAsync(category.Id);
        
        return CategoryMapping.MapToDto(category, itemsCount);
    }

    /// <summary>
    /// Получить категории по типу
    /// </summary>
    public async Task<IEnumerable<CategoryDto>> GetCategoriesByTypeAsync(string categoryType)
    {
        _logger.LogInformation("Getting categories by type: {CategoryType}", categoryType);
        
        var categories = await _unitOfWork.Categories.GetCategoriesByTypeAsync(categoryType);
        
        var categoryDtos = new List<CategoryDto>();
        foreach (var category in categories)
        {
            var itemsCount = await _unitOfWork.Categories.GetItemsCountAsync(category.Id);
            categoryDtos.Add(CategoryMapping.MapToDto(category, itemsCount));
        }
        
        return categoryDtos;
    }

    /// <summary>
    /// Создать новую категорию
    /// </summary>
    public async Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto)
    {
        _logger.LogInformation("Creating new category: {CategoryName}", categoryDto.Name);
        
        var category = new Category
        {
            Name = categoryDto.Name,
            CategoryType = categoryDto.CategoryType,
            Image = categoryDto.Image,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UseDefaultEula = false,
            RequireAcceptance = false,
            CheckinEmail = false
        };
        
        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation("Category created with ID: {CategoryId}", category.Id);
        
        return CategoryMapping.MapToDto(category, 0);
    }

    /// <summary>
    /// Обновить существующую категорию
    /// </summary>
    public async Task<CategoryDto> UpdateCategoryAsync(int id, CategoryDto categoryDto)
    {
        _logger.LogInformation("Updating category: {CategoryId}", id);
        
        var category = await _unitOfWork.Categories.GetByIdAsync((uint)id);
        
        if (category == null || category.DeletedAt != null)
        {
            _logger.LogWarning("Category not found for update: {CategoryId}", id);
            throw new NotFoundException("Category", id);
        }
        
        category.Name = categoryDto.Name;
        category.CategoryType = categoryDto.CategoryType;
        category.Image = categoryDto.Image;
        category.UpdatedAt = DateTime.UtcNow;
        
        _unitOfWork.Categories.Update(category);
        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation("Category updated: {CategoryId}", id);
        
        var itemsCount = await _unitOfWork.Categories.GetItemsCountAsync(category.Id);
        return CategoryMapping.MapToDto(category, itemsCount);
    }

    /// <summary>
    /// Удалить категорию (soft delete)
    /// </summary>
    public async Task DeleteCategoryAsync(int id)
    {
        _logger.LogInformation("Deleting category: {CategoryId}", id);
        
        var category = await _unitOfWork.Categories.GetByIdAsync((uint)id);
        
        if (category == null || category.DeletedAt != null)
        {
            _logger.LogWarning("Category not found for deletion: {CategoryId}", id);
            throw new NotFoundException("Category", id);
        }
        
        // Soft delete
        category.DeletedAt = DateTime.UtcNow;
        category.UpdatedAt = DateTime.UtcNow;
        
        _unitOfWork.Categories.Update(category);
        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation("Category deleted: {CategoryId}", id);
    }
}

