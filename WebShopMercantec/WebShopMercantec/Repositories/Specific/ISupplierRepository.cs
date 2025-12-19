using WebShopMercantec.Models;

namespace WebShopMercantec.Repositories.Specific;

public interface ISupplierRepository : IRepository<Supplier>
{
    Task<IEnumerable<Supplier>> GetAllActiveAsync();
    Task<Supplier?> GetActiveByIdAsync(uint id);
}

