using WebShopMercantec.Models;

namespace WebShopMercantec.Repositories.Specific;

/// <summary>
/// Product Repository Interface
/// Работает с таблицей Assets (в контексте магазина это Products)
/// 
/// ВАЖНО: В SnipeIT таблица называется Assets, но в контексте магазина это Products
/// Asset = физический актив (ноутбук, монитор, клавиатура и т.д.)
/// </summary>
public interface IProductRepository : IRepository<Asset>
{
    /// <summary>
    /// Получить доступные для заказа продукты
    /// Условия:
    /// - StatusId = 1 или 2 (Ready to Deploy)
    /// - Archived = false
    /// - DeletedAt = null
    /// - Requestable = 1 (можно запросить)
    /// </summary>
    Task<IEnumerable<Asset>> GetAvailableProductsAsync();
    
    /// <summary>
    /// Получить продукты по категории
    /// </summary>
    /// <param name="categoryId">ID категории (через Model.CategoryId)</param>
    Task<IEnumerable<Asset>> GetByCategoryAsync(int categoryId);
    
    /// <summary>
    /// Получить продукты по модели
    /// </summary>
    Task<IEnumerable<Asset>> GetByModelIdAsync(int modelId);
    
    /// <summary>
    /// Получить продукты по производителю
    /// Производитель связан через Model
    /// </summary>
    Task<IEnumerable<Asset>> GetByManufacturerAsync(int manufacturerId);
    
    /// <summary>
    /// Получить продукты по статусу
    /// </summary>
    Task<IEnumerable<Asset>> GetByStatusIdAsync(int statusId);
    
    /// <summary>
    /// Получить продукты по локации
    /// </summary>
    Task<IEnumerable<Asset>> GetByLocationIdAsync(int locationId);
    
    /// <summary>
    /// Получить продукты назначенные конкретному пользователю
    /// </summary>
    Task<IEnumerable<Asset>> GetAssignedToUserAsync(int userId);
    
    /// <summary>
    /// Поиск продуктов по названию или asset tag
    /// </summary>
    Task<IEnumerable<Asset>> SearchProductsAsync(string searchTerm);
    
    /// <summary>
    /// Получить продукты с пагинацией и фильтрами
    /// Самый мощный метод для каталога магазина
    /// </summary>
    Task<(IEnumerable<Asset> Products, int TotalCount)> GetProductsPagedAsync(
        int pageNumber,
        int pageSize,
        int? categoryId = null,
        int? manufacturerId = null,
        int? statusId = null,
        string? searchTerm = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? availableOnly = true);
    
    /// <summary>
    /// Проверить доступность продукта для заказа
    /// Проверяет статус, archived, и не назначен ли кому-то
    /// </summary>
    Task<bool> IsAvailableForCheckoutAsync(uint assetId);
    
    /// <summary>
    /// Получить продукт по Asset Tag (уникальный идентификатор)
    /// </summary>
    Task<Asset?> GetByAssetTagAsync(string assetTag);
    
    /// <summary>
    /// Получить архивированные продукты
    /// </summary>
    Task<IEnumerable<Asset>> GetArchivedProductsAsync();
    
    /// <summary>
    /// Получить продукты, требующие обслуживания
    /// Проверяет warranty, maintenance dates
    /// </summary>
    Task<IEnumerable<Asset>> GetProductsRequiringMaintenanceAsync();
}