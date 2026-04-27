using WebShopMercantec.Models;
using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Mapping;

public static class ProductMapping
{
        // main mapping: from asset with full relational data
    public static ProductDto MapFromDetails(AssetWithDetails details)
    {
        var (asset, model, category, manufacturer, statusLabel, location, supplier) = details;

        return new ProductDto
        {
            Id = (int)asset.Id,
            Name = asset.Name ?? "Unknown Product",
            AssetTag = asset.AssetTag ?? "N/A",
            Image = asset.Image,
            ModelId = asset.ModelId,
            ModelName = model?.Name,
            ModelNumber = model?.ModelNumber,
            Serial = asset.Serial,
            StatusId = asset.StatusId,
            StatusLabel = statusLabel?.Name ?? "Unknown",
            CategoryName = category?.Name ?? "Uncategorized",
            ManufacturerId = manufacturer != null ? (int?)manufacturer.Id : null,
            ManufacturerName = manufacturer?.Name,
            LocationId = asset.LocationId,
            LocationName = location?.Name,
            SupplierId = asset.SupplierId,
            SupplierName = supplier?.Name,
            CompanyId = (int?)asset.CompanyId,
            Notes = asset.Notes,
            PurchaseCost = asset.PurchaseCost,
            Price = asset.PurchaseCost ?? 0m,
            OrderNumber = asset.OrderNumber,
            RtdLocationId = asset.RtdLocationId,
            AssignedTo = asset.AssignedTo,
            AssignedType = asset.AssignedType,
            IsAvailable = IsAssetAvailableStrict(asset, model, statusLabel),
            Requestable = asset.Requestable == 1,
            Archived = asset.Archived,
            WarrantyMonths = asset.WarrantyMonths,
            PurchaseDate = asset.PurchaseDate.HasValue
                ? DateTime.Parse(asset.PurchaseDate.Value.ToString("yyyy-MM-dd"))
                : null,
            AssetEolDate = asset.AssetEolDate.HasValue
                ? DateTime.Parse(asset.AssetEolDate.Value.ToString("yyyy-MM-dd"))
                : null,
            LastCheckout = asset.LastCheckout,
            LastCheckin = asset.LastCheckin,
            ExpectedCheckin = asset.ExpectedCheckin.HasValue
                ? DateTime.Parse(asset.ExpectedCheckin.Value.ToString("yyyy-MM-dd"))
                : null,
            CreatedAt = asset.CreatedAt,
            UpdatedAt = asset.UpdatedAt
        };
    }

    public static IEnumerable<ProductDto> MapFromDetailsList(IEnumerable<AssetWithDetails> detailsList)
    {
        return detailsList.Select(MapFromDetails);
    }

    // legacy mapping: from raw asset entity without joins
    public static ProductDto MapAssetToDto(Asset asset)
    {
        return new ProductDto
        {
            Id = (int)asset.Id,
            Name = asset.Name ?? "Unknown Product",
            AssetTag = asset.AssetTag ?? "N/A",
            Image = asset.Image,
            ModelId = asset.ModelId,
            ModelName = null,
            Serial = asset.Serial,
            StatusId = asset.StatusId,
            StatusLabel = asset.StatusId.HasValue ? $"Status {asset.StatusId}" : "Unknown",
            CategoryName = "Unknown",
            Notes = asset.Notes,
            PurchaseCost = asset.PurchaseCost,
            Price = asset.PurchaseCost ?? 0m,
            OrderNumber = asset.OrderNumber,
            ManufacturerId = null,
            ManufacturerName = null,
            ModelNumber = null,
            LocationId = asset.LocationId,
            // requires model/status label to safely determine true availability
            IsAvailable = false,
            Requestable = asset.Requestable == 1,
            Archived = asset.Archived,
            CreatedAt = asset.CreatedAt,
            UpdatedAt = asset.UpdatedAt
        };
    }

    // business logic for 'is deployable' combining asset, model and status label
    private static bool IsAssetAvailableStrict(Asset asset, Model? model, StatusLabel? statusLabel)
    {
        return asset.DeletedAt == null
               && asset.AssignedTo == null
               && asset.Requestable == 1
               && model is { Requestable: 1, DeletedAt: null }
               && statusLabel is { Deployable: true, DeletedAt: null };
    }

    public static IEnumerable<ProductDto> MapAssetsToDtos(IEnumerable<Asset> assets)
    {
        return assets.Select(MapAssetToDto);
    }

    // --- ACCESSORIES ---

    // accessory mapping with full relations
    public static AccessoryDto MapFromDetails(AccessoryWithDetails details)
    {
        var (accessory, category, manufacturer, location, supplier) = details;

        return new AccessoryDto
        {
            Id = (int)accessory.Id,
            Name = accessory.Name ?? "Unknown Accessory",
            CategoryId = accessory.CategoryId,
            CategoryName = category?.Name,
            Qty = accessory.Qty,
            Requestable = accessory.Requestable,
            LocationId = accessory.LocationId,
            LocationName = location?.Name,
            PurchaseDate = accessory.PurchaseDate.HasValue
                ? DateTime.Parse(accessory.PurchaseDate.Value.ToString("yyyy-MM-dd"))
                : null,
            PurchaseCost = accessory.PurchaseCost,
            OrderNumber = accessory.OrderNumber,
            CompanyId = (int?)accessory.CompanyId,
            MinAmt = accessory.MinAmt,
            ManufacturerId = accessory.ManufacturerId,
            ManufacturerName = manufacturer?.Name,
            ModelNumber = accessory.ModelNumber,
            Image = accessory.Image,
            SupplierId = accessory.SupplierId,
            SupplierName = supplier?.Name,
            Notes = accessory.Notes,
            CreatedAt = accessory.CreatedAt,
            UpdatedAt = accessory.UpdatedAt
        };
    }

    // raw accessory mapping
    public static AccessoryDto MapAccessoryToDto(Accessory accessory)
    {
        return new AccessoryDto
        {
            Id = (int)accessory.Id,
            Name = accessory.Name ?? "Unknown Accessory",
            CategoryId = accessory.CategoryId,
            CategoryName = null,
            Qty = accessory.Qty,
            Requestable = accessory.Requestable,
            LocationId = accessory.LocationId,
            LocationName = null,
            PurchaseDate = accessory.PurchaseDate.HasValue 
                ? DateTime.Parse(accessory.PurchaseDate.Value.ToString("yyyy-MM-dd")) 
                : null,
            PurchaseCost = accessory.PurchaseCost,
            OrderNumber = accessory.OrderNumber,
            CompanyId = (int?)accessory.CompanyId,
            MinAmt = accessory.MinAmt,
            ManufacturerId = accessory.ManufacturerId,
            ManufacturerName = null,
            ModelNumber = accessory.ModelNumber,
            Image = accessory.Image,
            SupplierId = accessory.SupplierId,
            SupplierName = null,
            Notes = accessory.Notes,
            CreatedAt = accessory.CreatedAt,
            UpdatedAt = accessory.UpdatedAt
        };
    }

    public static IEnumerable<AccessoryDto> MapAccessoriesToDtos(IEnumerable<Accessory> accessories)
    {
        return accessories.Select(MapAccessoryToDto);
    }
}
