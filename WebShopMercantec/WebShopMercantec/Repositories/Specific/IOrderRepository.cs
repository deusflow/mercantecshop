using WebShopMercantec.Models;

namespace WebShopMercantec.Repositories.Specific;

public interface IOrderRepository : IRepository<CheckoutRequest>
{
    
    Task<IEnumerable<CheckoutRequest>> GetUserOrdersAsync(int userId);
    
    
    Task<(IEnumerable<CheckoutRequest> Orders, int TotalCount)> GetUserOrdersPagedAsync(
        int userId, 
        int pageNumber, 
        int pageSize);
    
    
    Task<IEnumerable<CheckoutRequest>> GetPendingOrdersAsync();
    
    
    Task<IEnumerable<CheckoutRequest>> GetFulfilledOrdersAsync();
    
    
    Task<IEnumerable<CheckoutRequest>> GetCanceledOrdersAsync();
    
    
    
    
    
    Task<(IEnumerable<CheckoutRequest> Orders, int TotalCount)> GetOrdersByStatusPagedAsync(
        string status, 
        int pageNumber, 
        int pageSize);
    
    
    Task<(IEnumerable<CheckoutRequest> Orders, int TotalCount)> GetAllOrdersPagedAsync(
        int pageNumber,
        int pageSize,
        string? status = null,
        int? userId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null);
    
    
    Task<IEnumerable<CheckoutRequest>> GetOrdersForAssetAsync(int assetId);
    
    
    Task<IEnumerable<CheckoutRequest>> GetOrdersForAccessoryAsync(int accessoryId);
    
    
    Task<bool> HasActivePendingOrderAsync(int userId, int requestableId, string requestableType);
    
    
    Task<int> GetUserOrderCountAsync(int userId);
    
    
    Task<int> GetPendingOrderCountAsync();
    
    
    Task<IEnumerable<CheckoutRequest>> GetOrdersByDateRangeAsync(
        DateTime fromDate, 
        DateTime toDate);
    
    
    Task<IEnumerable<CheckoutRequest>> GetRecentOrdersAsync(int count = 10);
}