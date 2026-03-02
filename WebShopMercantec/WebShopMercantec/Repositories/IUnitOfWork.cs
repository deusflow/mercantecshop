using WebShopMercantec.Models;
using WebShopMercantec.Repositories.Specific;

namespace WebShopMercantec.Repositories;

public interface IUnitOfWork : IDisposable
{
    // === REPOSITORIES ===
    IUserRepository Users { get; }
    IProductRepository Products { get; }
    IOrderRepository Orders { get; }
    IAccessoryRepository Accessories { get; }
    ICategoryRepository Categories { get; }
    IManufacturerRepository Manufacturers { get; }
    ISupplierRepository Suppliers { get; }
    ILocationRepository Locations { get; }
    IStatusLabelRepository StatusLabels { get; }

    /// <summary>
    /// Direct access to EF Core context for WebShop-specific tables
    /// (CreditTransactions, RefreshTokens, WebShopUserCredits)
    /// </summary>
    SnipeItContext Context { get; }

    // === MANAGEMENT ===
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}