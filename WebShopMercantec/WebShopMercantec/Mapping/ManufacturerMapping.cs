using WebShopMercantec.Models;
using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Mapping;

public static class ManufacturerMapping
{
    public static ManufacturerDto MapToDto(Manufacturer manufacturer, int productsCount = 0)
    {
        return new ManufacturerDto
        {
            Id = (int)manufacturer.Id,
            Name = manufacturer.Name ?? "Unknown",
            Url = manufacturer.Url,
            SupportUrl = manufacturer.SupportUrl,
            SupportPhone = manufacturer.SupportPhone,
            SupportEmail = manufacturer.SupportEmail,
            ProductsCount = productsCount
        };
    }

    public static IEnumerable<ManufacturerDto> MapToDtos(
        IEnumerable<Manufacturer> manufacturers,
        Func<uint, int>? getProductsCount = null)
    {
        return manufacturers.Select(m => MapToDto(m, getProductsCount?.Invoke(m.Id) ?? 0));
    }
}

