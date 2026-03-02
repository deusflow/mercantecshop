using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Services;

public interface ICreditService
{
    Task<decimal> GetBalanceAsync(uint userId);
    Task<TransactionDto> AddCreditsAsync(uint userId, decimal amount, string reason, int? relatedOrderId = null);
    Task<TransactionDto> DeductCreditsAsync(uint userId, decimal amount, string reason, int? relatedOrderId = null);
    Task<bool> HasSufficientCreditsAsync(uint userId, decimal amount);
    Task<IEnumerable<TransactionDto>> GetTransactionHistoryAsync(uint userId, int page = 1, int pageSize = 20);
}

