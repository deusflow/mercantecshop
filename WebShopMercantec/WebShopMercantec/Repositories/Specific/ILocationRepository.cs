using WebShopMercantec.Models;
namespace WebShopMercantec.Repositories.Specific;
public interface ILocationRepository : IRepository<Location>
{
    Task<IEnumerable<Location>> GetAllActiveAsync();
    Task<Location?> GetActiveByIdAsync(uint id);
}
