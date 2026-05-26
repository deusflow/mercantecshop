using WebShopMercantec.Shared.DTOs;
using WebShopMercantec.Repositories;
using WebShopMercantec.Exceptions;
using WebShopMercantec.Mapping;

namespace WebShopMercantec.Services;

public class LocationService : ILocationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LocationService> _logger;

    public LocationService(IUnitOfWork unitOfWork, ILogger<LocationService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IEnumerable<LocationDto>> GetAllLocationsAsync()
    {
        _logger.LogInformation("Getting all locations");
        var locations = await _unitOfWork.Locations.GetAllActiveAsync();
        var locationDtos = LocationMapping.MapToDtos(locations).ToList();
        _logger.LogInformation("Found {Count} locations", locationDtos.Count);
        return locationDtos;
    }

    public async Task<LocationDto?> GetLocationByIdAsync(int id)
    {
        _logger.LogInformation("Getting location with ID: {LocationId}", id);
        var location = await _unitOfWork.Locations.GetActiveByIdAsync((uint)id);
        
        if (location == null)
        {
            _logger.LogWarning("Location not found: {LocationId}", id);
            throw new NotFoundException("Location", id);
        }
        
        return LocationMapping.MapToDto(location);
    }
}

