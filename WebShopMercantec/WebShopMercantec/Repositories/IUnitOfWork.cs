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

    
    SnipeItContext Context { get; }

    // === MANAGEMENT ===
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}