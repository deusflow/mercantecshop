using WebShopMercantec.Models;

namespace WebShopMercantec.Repositories.Specific;

/// <summary>
/// Order Repository Interface
/// Работает с таблицей CheckoutRequests (заказы пользователей)
/// 
/// ВАЖНО: В SnipeIT это CheckoutRequest, но в контексте магазина это Order
/// CheckoutRequest = запрос на получение актива = Заказ в магазине
/// 
/// СТАТУСЫ ЗАКАЗА (определяются по датам):
/// - Pending: CreatedAt есть, но FulfilledAt и CanceledAt = null
/// - Fulfilled: FulfilledAt != null (выполнен)
/// - Canceled: CanceledAt != null (отменен)
/// </summary>
public interface IOrderRepository : IRepository<CheckoutRequest>
{
    /// <summary>
    /// Получить заказы конкретного пользователя
    /// </summary>
    Task<IEnumerable<CheckoutRequest>> GetUserOrdersAsync(int userId);
    
    /// <summary>
    /// Получить заказы пользователя с пагинацией
    /// </summary>
    Task<(IEnumerable<CheckoutRequest> Orders, int TotalCount)> GetUserOrdersPagedAsync(
        int userId, 
        int pageNumber, 
        int pageSize);
    
    /// <summary>
    /// Получить все ожидающие заказы (Pending)
    /// Для админ-панели - заказы, которые нужно обработать
    /// </summary>
    Task<IEnumerable<CheckoutRequest>> GetPendingOrdersAsync();
    
    /// <summary>
    /// Получить выполненные заказы (Fulfilled)
    /// </summary>
    Task<IEnumerable<CheckoutRequest>> GetFulfilledOrdersAsync();
    
    /// <summary>
    /// Получить отмененные заказы (Canceled)
    /// </summary>
    Task<IEnumerable<CheckoutRequest>> GetCanceledOrdersAsync();
    
    /// <summary>
    /// Получить заказы по статусу с пагинацией
    /// </summary>
    /// <param name="status">Pending, Fulfilled, Canceled</param>
    /// <param name="pageNumber">Номер страницы</param>
    /// <param name="pageSize">Размер страницы</param>
    Task<(IEnumerable<CheckoutRequest> Orders, int TotalCount)> GetOrdersByStatusPagedAsync(
        string status, 
        int pageNumber, 
        int pageSize);
    
    /// <summary>
    /// Получить все заказы с пагинацией и фильтрами
    /// Для админ-панели
    /// </summary>
    Task<(IEnumerable<CheckoutRequest> Orders, int TotalCount)> GetAllOrdersPagedAsync(
        int pageNumber,
        int pageSize,
        string? status = null,
        int? userId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null);
    
    /// <summary>
    /// Получить заказы для конкретного товара (Asset)
    /// Показывает историю заказов товара
    /// </summary>
    Task<IEnumerable<CheckoutRequest>> GetOrdersForAssetAsync(int assetId);
    
    /// <summary>
    /// Получить заказы для конкретного аксессуара
    /// </summary>
    Task<IEnumerable<CheckoutRequest>> GetOrdersForAccessoryAsync(int accessoryId);
    
    /// <summary>
    /// Проверить, есть ли у пользователя активный заказ на данный товар
    /// Активный = Pending (еще не выполнен и не отменен)
    /// </summary>
    Task<bool> HasActivePendingOrderAsync(int userId, int requestableId, string requestableType);
    
    /// <summary>
    /// Подсчитать количество заказов пользователя
    /// </summary>
    Task<int> GetUserOrderCountAsync(int userId);
    
    /// <summary>
    /// Подсчитать количество ожидающих заказов
    /// Для отображения в админ-панели
    /// </summary>
    Task<int> GetPendingOrderCountAsync();
    
    /// <summary>
    /// Получить заказы за период времени
    /// Для статистики и отчетов
    /// </summary>
    Task<IEnumerable<CheckoutRequest>> GetOrdersByDateRangeAsync(
        DateTime fromDate, 
        DateTime toDate);
    
    /// <summary>
    /// Получить последние N заказов
    /// Для dashboard
    /// </summary>
    Task<IEnumerable<CheckoutRequest>> GetRecentOrdersAsync(int count = 10);
}