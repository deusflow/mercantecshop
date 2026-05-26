using WebShopMercantec.Models;
namespace WebShopMercantec.Repositories.Specific;
public interface IStatusLabelRepository : IRepository<StatusLabel>
{
    Task<IEnumerable<StatusLabel>> GetAllActiveAsync();
    Task<StatusLabel?> GetActiveByIdAsync(uint id);
    Task<int> GetAssetsCountAsync(uint statusId);
    Task<IEnumerable<StatusLabel>> GetDeployableStatusesAsync();
}
