using WebShopMercantec.Models;
namespace WebShopMercantec.Repositories.Specific;
public interface IManufacturerRepository : IRepository<Manufacturer>
{
    Task<IEnumerable<Manufacturer>> GetAllActiveAsync();
    Task<Manufacturer?> GetActiveByIdAsync(uint id);
    Task<int> GetProductsCountAsync(uint manufacturerId);
}
