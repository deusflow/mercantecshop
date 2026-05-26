using WebShopMercantec.Models;
using WebShopMercantec.Shared.DTOs;
namespace WebShopMercantec.Mapping;
public static class LocationMapping
{
    public static LocationDto MapToDto(Location location)
    {
        return new LocationDto
        {
            Id = (int)location.Id,
            Name = location.Name ?? "Unknown",
            Address = location.Address,
            Address2 = location.Address2,
            City = location.City,
            State = location.State,
            Country = location.Country,
            Zip = location.Zip
        };
    }
    public static IEnumerable<LocationDto> MapToDtos(IEnumerable<Location> locations)
    {
        return locations.Select(MapToDto);
    }
}
