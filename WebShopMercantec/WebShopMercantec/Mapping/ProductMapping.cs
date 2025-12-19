using WebShopMercantec.Models;
using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Mapping;

/// <summary>
/// Маппинг для Product (Asset и Accessory) -> ProductDto/AccessoryDto
/// Централизованное место для преобразования Entity в DTO
/// </summary>
public static class ProductMapping
{
    /// <summary>
    /// Преобразовать Asset в ProductDto
    /// </summary>
    public static ProductDto MapAssetToDto(Asset asset)
    {
        // TODO: В будущем получать связанные данные через репозитории
        // (Model, Category, Manufacturer, StatusLabel)
        
        return new ProductDto
        {
            Id = (int)asset.Id,
            Name = asset.Name ?? "Unknown Product",
            AssetTag = asset.AssetTag ?? "N/A",
            Image = asset.Image,
            ModelId = asset.ModelId,
            ModelName = null, // TODO: получить из Model
            Serial = asset.Serial,
            StatusId = asset.StatusId,
            StatusLabel = asset.StatusId.HasValue ? $"Status {asset.StatusId}" : "Unknown",
            CategoryName = "Unknown", // TODO: получить из Category через Model
            Notes = asset.Notes,
            PurchaseCost = asset.PurchaseCost,
            Price = asset.PurchaseCost ?? 0m,
            OrderNumber = asset.OrderNumber,
            ManufacturerId = null, // TODO: получить из Model
            ManufacturerName = null,
            ModelNumber = null,
            LocationId = asset.LocationId
        };
    }

    /// <summary>
    /// Преобразовать список Assets в список ProductDto
    /// </summary>
    public static IEnumerable<ProductDto> MapAssetsToDtos(IEnumerable<Asset> assets)
    {
        return assets.Select(MapAssetToDto);
    }

    /// <summary>
    /// Преобразовать Accessory в AccessoryDto
    /// </summary>
    public static AccessoryDto MapAccessoryToDto(Accessory accessory)
    {
        return new AccessoryDto
        {
            Id = (int)accessory.Id,
            Name = accessory.Name ?? "Unknown Accessory",
            CategoryId = accessory.CategoryId,
            CategoryName = null, // TODO: получить из Category
            Qty = accessory.Qty,
            Requestable = accessory.Requestable,
            LocationId = accessory.LocationId,
            LocationName = null, // TODO: получить из Location
            PurchaseDate = accessory.PurchaseDate.HasValue 
                ? DateTime.Parse(accessory.PurchaseDate.Value.ToString("yyyy-MM-dd")) 
                : null,
            PurchaseCost = accessory.PurchaseCost,
            OrderNumber = accessory.OrderNumber,
            CompanyId = (int?)accessory.CompanyId,
            MinAmt = accessory.MinAmt,
            ManufacturerId = accessory.ManufacturerId,
            ManufacturerName = null, // TODO: получить из Manufacturer
            ModelNumber = accessory.ModelNumber,
            Image = accessory.Image,
            SupplierId = accessory.SupplierId,
            SupplierName = null, // TODO: получить из Supplier
            Notes = accessory.Notes,
            CreatedAt = accessory.CreatedAt,
            UpdatedAt = accessory.UpdatedAt
        };
    }

    /// <summary>
    /// Преобразовать список Accessories в список AccessoryDto
    /// </summary>
    public static IEnumerable<AccessoryDto> MapAccessoriesToDtos(IEnumerable<Accessory> accessories)
    {
        return accessories.Select(MapAccessoryToDto);
    }
}

