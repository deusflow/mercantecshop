using WebShopMercantec.Shared.DTOs;
namespace WebShopMercantec.Services;
public interface IStatusLabelService
{
    Task<IEnumerable<StatusLabelDto>> GetAllStatusLabelsAsync();
    Task<StatusLabelDto?> GetStatusLabelByIdAsync(int id);
    Task<IEnumerable<StatusLabelDto>> GetDeployableStatusesAsync();
}
