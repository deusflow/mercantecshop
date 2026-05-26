namespace WebShopMercantec.Models;

public record AssetWithDetails(
    Asset Asset,
    Model? Model,
    Category? Category,
    Manufacturer? Manufacturer,
    StatusLabel? StatusLabel,
    Location? Location,
    Supplier? Supplier);

public record AccessoryWithDetails(
    Accessory Accessory,
    Category? Category,
    Manufacturer? Manufacturer,
    Location? Location,
    Supplier? Supplier);

