namespace WebShopMercantec.Models;

/// <summary>
/// Projection record для Asset со связанными данными.
/// Используется для загрузки данных из нескольких таблиц одним запросом
/// (без navigation properties в scaffolded моделях).
/// </summary>
public record AssetWithDetails(
    Asset Asset,
    Model? Model,
    Category? Category,
    Manufacturer? Manufacturer,
    StatusLabel? StatusLabel,
    Location? Location,
    Supplier? Supplier);

/// <summary>
/// Projection record для Accessory со связанными данными.
/// </summary>
public record AccessoryWithDetails(
    Accessory Accessory,
    Category? Category,
    Manufacturer? Manufacturer,
    Location? Location,
    Supplier? Supplier);

