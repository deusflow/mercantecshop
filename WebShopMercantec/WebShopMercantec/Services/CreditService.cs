using WebShopMercantec.Exceptions;
using WebShopMercantec.Models;
using WebShopMercantec.Repositories;
using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Services;

public class CreditService : ICreditService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreditService> _logger;

    public CreditService(IUnitOfWork unitOfWork, ILogger<CreditService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<decimal> GetBalanceAsync(uint userId)
    {
        var credits = await GetOrCreateCreditsAsync(userId, saveChanges: true);
        return credits.AvailableCredits;
    }

    public async Task<bool> HasSufficientCreditsAsync(uint userId, decimal amount)
    {
        var balance = await GetBalanceAsync(userId);
        return balance >= amount;
    }

    public async Task<TransactionDto> AddCreditsAsync(uint userId, decimal amount, string reason, int? relatedOrderId = null, bool saveChanges = true)
    {
        if (amount <= 0) throw new BadRequestException("Credit amount must be positive");

        var credits = await GetOrCreateCreditsAsync(userId, saveChanges);
        var before = credits.AvailableCredits;

        credits.AvailableCredits += amount;
        credits.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Context.WebShopUserCredits.Update(credits);

        var tx = await RecordTransactionAsync(userId, amount, "credit", reason, before, credits.AvailableCredits, relatedOrderId);
        
        if (saveChanges)
            await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Added {Amount} credits to user {UserId}. Balance: {Balance}", amount, userId, credits.AvailableCredits);
        return MapTransaction(tx);
    }

    public async Task<TransactionDto> DeductCreditsAsync(uint userId, decimal amount, string reason, int? relatedOrderId = null, bool saveChanges = true)
    {
        if (amount <= 0) throw new BadRequestException("Debit amount must be positive");

        var credits = await GetOrCreateCreditsAsync(userId, saveChanges);

        if (credits.AvailableCredits < amount)
            throw new InsufficientCreditsException(amount, credits.AvailableCredits);

        var before = credits.AvailableCredits;
        credits.AvailableCredits -= amount;
        credits.TotalSpent += amount;
        credits.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Context.WebShopUserCredits.Update(credits);

        var tx = await RecordTransactionAsync(userId, -amount, "debit", reason, before, credits.AvailableCredits, relatedOrderId);
        
        if (saveChanges)
            await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Deducted {Amount} credits from user {UserId}. Balance: {Balance}", amount, userId, credits.AvailableCredits);
        return MapTransaction(tx);
    }

    public async Task<IEnumerable<TransactionDto>> GetTransactionHistoryAsync(uint userId, int page = 1, int pageSize = 20)
    {
        var txs = await _unitOfWork.Context.CreditTransactions
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return txs.Select(MapTransaction);
    }

    // ─ Helpers 

    private async Task<WebShopUserCredits> GetOrCreateCreditsAsync(uint userId, bool saveChanges = true)
    {
        var credits = _unitOfWork.Context.WebShopUserCredits.Local
            .FirstOrDefault(c => c.UserId == userId) 
            ?? await _unitOfWork.Context.WebShopUserCredits.FirstOrDefaultAsync(c => c.UserId == userId);

        if (credits != null) return credits;

        credits = new WebShopUserCredits
        {
            UserId = userId,
            AvailableCredits = 0m,
            TotalSpent = 0m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _unitOfWork.Context.WebShopUserCredits.AddAsync(credits);
        
        if (saveChanges)
            await _unitOfWork.SaveChangesAsync();
            
        return credits;
    }

    private async Task<CreditTransaction> RecordTransactionAsync(
        uint userId, decimal amount, string type, string reason,
        decimal balanceBefore, decimal balanceAfter, int? relatedOrderId)
    {
        var tx = new CreditTransaction
        {
            UserId = userId,
            Amount = amount,
            Type = type,
            Reason = reason,
            BalanceBefore = balanceBefore,
            BalanceAfter = balanceAfter,
            RelatedOrderId = relatedOrderId,
            CreatedAt = DateTime.UtcNow
        };
        await _unitOfWork.Context.CreditTransactions.AddAsync(tx);
        return tx;
    }

    private static TransactionDto MapTransaction(CreditTransaction tx) => new()
    {
        Id = tx.Id,
        UserId = (int)tx.UserId,
        Amount = tx.Amount,
        Type = tx.Type,
        Description = tx.Reason,
        BalanceBefore = tx.BalanceBefore,
        BalanceAfter = tx.BalanceAfter,
        Status = "Completed",
        CreatedAt = tx.CreatedAt,
        RelatedOrderId = tx.RelatedOrderId
    };
}

