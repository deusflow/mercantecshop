using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Services;

public interface IManufacturerService
{
    Task<IEnumerable<ManufacturerDto>> GetAllManufacturersAsync();
    Task<ManufacturerDto?> GetManufacturerByIdAsync(int id);
}

