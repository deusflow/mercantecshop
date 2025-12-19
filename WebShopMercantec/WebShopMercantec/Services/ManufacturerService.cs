using WebShopMercantec.Shared.DTOs;
using WebShopMercantec.Repositories;
using WebShopMercantec.Exceptions;
using WebShopMercantec.Mapping;

namespace WebShopMercantec.Services;

public class ManufacturerService : IManufacturerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ManufacturerService> _logger;

    public ManufacturerService(IUnitOfWork unitOfWork, ILogger<ManufacturerService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IEnumerable<ManufacturerDto>> GetAllManufacturersAsync()
    {
        _logger.LogInformation("Getting all manufacturers");
        var manufacturers = await _unitOfWork.Manufacturers.GetAllActiveAsync();
        
        var manufacturerDtos = new List<ManufacturerDto>();
        foreach (var manufacturer in manufacturers)
        {
            var productsCount = await _unitOfWork.Manufacturers.GetProductsCountAsync(manufacturer.Id);
            manufacturerDtos.Add(ManufacturerMapping.MapToDto(manufacturer, productsCount));
        }
        
        _logger.LogInformation("Found {Count} manufacturers", manufacturerDtos.Count);
        return manufacturerDtos;
    }

    public async Task<ManufacturerDto?> GetManufacturerByIdAsync(int id)
    {
        _logger.LogInformation("Getting manufacturer with ID: {ManufacturerId}", id);
        var manufacturer = await _unitOfWork.Manufacturers.GetActiveByIdAsync((uint)id);
        
        if (manufacturer == null)
        {
            _logger.LogWarning("Manufacturer not found: {ManufacturerId}", id);
            throw new NotFoundException("Manufacturer", id);
        }
        
        var productsCount = await _unitOfWork.Manufacturers.GetProductsCountAsync(manufacturer.Id);
        return ManufacturerMapping.MapToDto(manufacturer, productsCount);
    }
}
