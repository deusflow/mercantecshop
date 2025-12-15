using WebShopMercantec.Models;

namespace WebShopMercantec.Repositories.Specific;

/// <summary>
/// Accessory Repository Interface
/// Работает с таблицей Accessories (аксессуары: мыши, клавиатуры, кабели и т.д.)
/// 
/// ОТЛИЧИЕ ОТ ASSETS:
/// - Asset - единичный предмет (1 ноутбук, 1 монитор)
/// - Accessory - может быть несколько штук (qty)
/// - Accessory можно checkout несколько раз разным пользователям
/// </summary>
public interface IAccessoryRepository : IRepository<Accessory>
{
    /// <summary>
    /// Получить доступные аксессуары
    /// Условия:
    /// - Requestable = true
    /// - Qty > 0 (есть в наличии)
    /// - DeletedAt = null
    /// </summary>
    Task<IEnumerable<Accessory>> GetAvailableAccessoriesAsync();
    
    /// <summary>
    /// Получить аксессуары по категории
    /// </summary>
    Task<IEnumerable<Accessory>> GetByCategoryIdAsync(int categoryId);
    
    /// <summary>
    /// Получить аксессуары по производителю
    /// </summary>
    Task<IEnumerable<Accessory>> GetByManufacturerIdAsync(int manufacturerId);
    
    /// <summary>
    /// Получить аксессуары по локации
    /// </summary>
    Task<IEnumerable<Accessory>> GetByLocationIdAsync(int locationId);
    
    /// <summary>
    /// Получить аксессуары по компании
    /// </summary>
    Task<IEnumerable<Accessory>> GetByCompanyIdAsync(uint companyId);
    
    /// <summary>
    /// Поиск аксессуаров по названию или номеру модели
    /// </summary>
    Task<IEnumerable<Accessory>> SearchAccessoriesAsync(string searchTerm);
    
    /// <summary>
    /// Получить аксессуары с пагинацией и фильтрами
    /// </summary>
    Task<(IEnumerable<Accessory> Accessories, int TotalCount)> GetAccessoriesPagedAsync(
        int pageNumber,
        int pageSize,
        int? categoryId = null,
        int? manufacturerId = null,
        string? searchTerm = null,
        bool? availableOnly = true);
    
    /// <summary>
    /// Проверить доступность аксессуара
    /// Проверяет qty, requestable, deleted
    /// </summary>
    Task<bool> IsAvailableAsync(uint accessoryId, int requestedQuantity = 1);
    
    /// <summary>
    /// Получить доступное количество аксессуара
    /// Возвращает Qty (общее количество)
    /// </summary>
    Task<int> GetAvailableQuantityAsync(uint accessoryId);
    
    /// <summary>
    /// Получить аксессуары с низким запасом
    /// Qty меньше или равно MinAmt (minimum amount)
    /// Для уведомлений и пополнения запасов
    /// </summary>
    Task<IEnumerable<Accessory>> GetLowStockAccessoriesAsync();
    
    /// <summary>
    /// Получить аксессуары без запасов
    /// Qty = 0
    /// </summary>
    Task<IEnumerable<Accessory>> GetOutOfStockAccessoriesAsync();
    
    /// <summary>
    /// Обновить количество аксессуара
    /// Используется при checkout/checkin
    /// </summary>
    Task<bool> UpdateQuantityAsync(uint accessoryId, int quantityChange);
}