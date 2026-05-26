using WebShopMercantec.Shared.DTOs;
namespace WebShopMercantec.Services;
public interface ILocationService
{
    Task<IEnumerable<LocationDto>> GetAllLocationsAsync();
    Task<LocationDto?> GetLocationByIdAsync(int id);
}
