using WebShopMercantec.Models;
using WebShopMercantec.Shared.DTOs;
using WebShopMercantec.Repositories;
using WebShopMercantec.Exceptions;
using WebShopMercantec.Mapping;

namespace WebShopMercantec.Services;

// Category service
// Uses the repository pattern and handles business logic

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(IUnitOfWork unitOfWork, ILogger<CategoryService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    // Get all active categories with item counts
    // Uses a batch query for counts (2 SQL calls instead of N+1)
    
    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        _logger.LogInformation("Getting all categories");
        
        var categories = await _unitOfWork.Categories.GetAllActiveCategoriesAsync();
        var counts = await _unitOfWork.Categories.GetAllItemsCountsBatchAsync();
        
        var categoryDtos = categories.Select(c => 
            CategoryMapping.MapToDto(c, counts.GetValueOrDefault(c.Id, 0))
        ).ToList();
        
        _logger.LogInformation("Found {Count} categories", categoryDtos.Count);
        return categoryDtos;
    }

    public async Task<IEnumerable<CategoryDto>> GetCatalogCategoriesAsync(bool includeHidden = false)
    {
        var categories = await _unitOfWork.Context.Categories
            .AsNoTracking()
            .Where(c => c.DeletedAt == null && c.CategoryType == "asset")
            .OrderBy(c => c.Name)
            .ToListAsync();

        var availableCounts = await GetAvailableAssetCountsByCategoryAsync();

        var mapped = categories
            .Select(c => CategoryMapping.MapToDto(c, availableCounts.GetValueOrDefault(c.Id, 0)))
            .ToList();

        if (includeHidden)
            return mapped;

        var explicitlyVisible = mapped.Where(c => c.ShowInCatalog).ToList();
        if (explicitlyVisible.Count > 0)
            return explicitlyVisible;

        // Backward-compatible fallback: if no explicit flags were saved yet,
        // show only categories that currently have at least one available device.
        return mapped.Where(c => c.ItemsCount > 0).ToList();
    }

    public async Task SetCategoryVisibilityAsync(int categoryId, bool visible)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync((uint)categoryId);
        if (category == null || category.DeletedAt != null)
            throw new NotFoundException("Category", categoryId);

        category.CheckinEmail = visible;
        category.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Categories.Update(category);
        await _unitOfWork.SaveChangesAsync();
    }

    // Get category by ID
    
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

    // Get categories by type
    
    public async Task<IEnumerable<CategoryDto>> GetCategoriesByTypeAsync(string categoryType)
    {
        _logger.LogInformation("Getting categories by type: {CategoryType}", categoryType);
        
        var categories = await _unitOfWork.Categories.GetCategoriesByTypeAsync(categoryType);
        var counts = await _unitOfWork.Categories.GetAllItemsCountsBatchAsync();
        
        return categories.Select(c => 
            CategoryMapping.MapToDto(c, counts.GetValueOrDefault(c.Id, 0))
        ).ToList();
    }

    // Create a new category
    
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
            CheckinEmail = categoryDto.ShowInCatalog
        };
        
        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation("Category created with ID: {CategoryId}", category.Id);
        
        return CategoryMapping.MapToDto(category, 0);
    }

    // Update an existing category
    
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
        category.CheckinEmail = categoryDto.ShowInCatalog;
        category.UpdatedAt = DateTime.UtcNow;
        
        _unitOfWork.Categories.Update(category);
        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation("Category updated: {CategoryId}", id);
        
        var itemsCount = await _unitOfWork.Categories.GetItemsCountAsync(category.Id);
        return CategoryMapping.MapToDto(category, itemsCount);
    }

    // Delete a category (soft delete)
    
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

    private async Task<Dictionary<uint, int>> GetAvailableAssetCountsByCategoryAsync()
    {
        return await (
            from model in _unitOfWork.Context.Models.AsNoTracking()
            where model.DeletedAt == null && model.CategoryId.HasValue && model.Requestable == 1
            join asset in _unitOfWork.Context.Assets.AsNoTracking()
                on (int?)model.Id equals asset.ModelId
            join status in _unitOfWork.Context.StatusLabels.AsNoTracking()
                on asset.StatusId equals (int?)status.Id
            where asset.DeletedAt == null
                  && asset.AssignedTo == null
                  && asset.Requestable == 1
                  && status.DeletedAt == null
                  && status.Deployable
            group asset by (uint)model.CategoryId!.Value
            into g
            select new { CategoryId = g.Key, Count = g.Count() }
        ).ToDictionaryAsync(x => x.CategoryId, x => x.Count);
    }
}
