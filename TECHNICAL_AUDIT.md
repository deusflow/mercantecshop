# 🔍 Технический аудит WebShopMercantec — Production-Ready MVP Plan

> **Дата аудита:** 2 марта 2026  
> **Стек:** .NET 9 / Blazor WASM / ASP.NET Web API / EF Core 9 / MariaDB (Snipe-IT)  
> **Текущий статус:** ~18% backend-а готово (инфраструктурный каркас)

---

## 📑 Содержание

1. [Текущее состояние и найденные проблемы](#1-текущее-состояние-и-найденные-проблемы)
2. [Архитектура: текущая vs целевая](#2-архитектура-текущая-vs-целевая)
3. [Структура папок проекта](#3-структура-папок-проекта)
4. [Поток запроса (Request Lifecycle)](#4-поток-запроса-request-lifecycle)
5. [Конкретные исправления с кодом](#5-конкретные-исправления-с-кодом)
6. [Поэтапный MVP Roadmap](#6-поэтапный-mvp-roadmap)
7. [Security рекомендации](#7-security-рекомендации)
8. [DevOps рекомендации](#8-devops-рекомендации)
9. [Архитектурные риски и как их избежать](#9-архитектурные-риски-и-как-их-избежать)

---

## 1. Текущее состояние и найденные проблемы

### ✅ Что уже сделано хорошо

| Компонент | Статус | Качество |
|-----------|--------|----------|
| Repository Pattern (IRepository<T> + Repository<T>) | ✅ Готов | Хорошо — generic + specific |
| Unit of Work | ✅ Готов | Средне — проблема с DI (см. ниже) |
| 6 контроллеров (Products, Categories, Manufacturers, Suppliers, Locations, StatusLabels) | ✅ Готов | Хорошо |
| 6 сервисов + интерфейсы | ✅ Готов | Средне — N+1, TODO-заглушки |
| Mapping слой (статические классы) | ✅ Готов | Плохо — TODO-null в ключевых полях |
| FluentValidation (7 валидаторов) | ✅ Готов | Хорошо |
| ErrorHandlingMiddleware + 6 custom exceptions | ✅ Готов | Хорошо |
| Serilog (console + file) | ✅ Готов | Хорошо |
| Swagger | ✅ Готов | Хорошо |
| DTOs в Shared проекте | ✅ Готов | Хорошо |
| EF Core + Pomelo (MariaDB) | ✅ Готов | Работает |

### 🔴 КРИТИЧЕСКИЕ проблемы (исправить НЕМЕДЛЕННО)

#### ПРОБЛЕМА 1: Пароль в Git-репозитории
- **Файл:** `code.txt` — содержит пароль `Merc2024!` в открытом виде
- **Файл:** `dump.txt` — вероятно тоже содержит чувствительные данные
- **Причина:** `code.txt` и `dump.txt` НЕ в `.gitignore`
- **Риск:** Любой с доступом к репо видит SSH/DB пароли
- **Действие:**
  1. Добавить в `.gitignore`
  2. `git rm --cached code.txt dump.txt`
  3. Очистить историю Git (`git filter-repo` или BFG Repo-Cleaner)
  4. **Сменить пароль на сервере 192.168.115.187**

#### ПРОБЛЕМА 2: Нет аутентификации
- **ВСЕ endpoint-ы публичные** — любой может вызвать POST/PUT/DELETE
- Нет JWT, нет cookies, нет никакой авторизации
- CategoryController позволяет CREATE/UPDATE/DELETE без auth

#### ПРОБЛЕМА 3: Нет CORS
- Blazor WASM клиент не сможет обращаться к API в production
- В dev режиме работает только потому что server + client на одном порту

#### ПРОБЛЕМА 4: ConnectionString только в Development
- `appsettings.json` не содержит ConnectionString
- При запуске в Production — crash при старте

### 🟡 СЕРЬЁЗНЫЕ проблемы (исправить до MVP)

#### ПРОБЛЕМА 5: N+1 запросов в CategoryService
```csharp
// CategoryService.cs — ТЕКУЩИЙ КОД (ПЛОХО):
foreach (var category in categories)
{
    var itemsCount = await _unitOfWork.Categories.GetItemsCountAsync(category.Id);
    categoryDtos.Add(CategoryMapping.MapToDto(category, itemsCount));
}
// При 50 категориях = 51 запрос к БД!
```

#### ПРОБЛЕМА 6: Маппинг не загружает связанные данные
```csharp
// ProductMapping.cs — ВСЕ связанные поля = null:
ModelName = null,        // TODO: получить из Model
CategoryName = "Unknown", // TODO: получить из Category через Model
ManufacturerName = null,  // TODO: получить из Model
LocationName = null,      // не маппится вообще
```
**Причина:** Репозиторий не делает `.Include()` для связанных таблиц.

#### ПРОБЛЕМА 7: UnitOfWork обходит DI
```csharp
// UnitOfWork.cs — создаёт репозитории через new (ПЛОХО):
_users ??= new UserRepository(_context);
// Должен получать через DI (конструктор)
```

#### ПРОБЛЕМА 8: uint/int несоответствие
- Scaffolded модели используют `uint Id` (MySQL UNSIGNED INT)
- DTOs и контроллеры используют `int`
- Кастинг `(uint)id` и `(int)asset.Id` разбросан по коду

#### ПРОБЛЕМА 9: Тестовый endpoint в Program.cs
```csharp
// Program.cs — прямой доступ к DbContext минуя архитектуру:
app.MapGet("/test-assets", async (SnipeItContext db) => ...);
```

#### ПРОБЛЕМА 10: Async anti-pattern
```csharp
// ProductService.cs — обёртка sync метода в Task.FromResult:
private Task<ProductDto> MapAssetToDtoAsync(Asset asset)
{
    return Task.FromResult(ProductMapping.MapAssetToDto(asset));
}
```

### 🟢 НЕЗНАЧИТЕЛЬНЫЕ проблемы

| # | Проблема | Файл |
|---|----------|------|
| 11 | Blazor Interactive WebAssembly включён, но клиентских страниц нет | Program.cs |
| 12 | `SerilogConfiguration.cs` не используется (Serilog настроен inline в Program.cs) | Configuration/ |
| 13 | `appsettings.Development.json` содержит комментарии (невалидный JSON) | appsettings.Development.json |

---

## 2. Архитектура: текущая vs целевая

### Текущая архитектура
```
┌─────────────────────────────────────────────────┐
│                 Controllers                      │
│  (Products, Categories, Manufacturers...)        │
├──────────────────────┬──────────────────────────┤
│       Services       │     FluentValidation     │
│  (ProductService...) │   (7 validators)         │
├──────────────────────┴──────────────────────────┤
│              Manual Mapping Layer                │
│  (ProductMapping, CategoryMapping...)            │
├─────────────────────────────────────────────────┤
│        Repository + Unit of Work                 │
│  (IRepository<T>, IUnitOfWork, 9 repos)         │
├─────────────────────────────────────────────────┤
│   SnipeItContext (EF Core, scaffolded models)    │
├─────────────────────────────────────────────────┤
│            MariaDB (Snipe-IT DB)                 │
│        через SSH туннель (порт 3307)            │
└─────────────────────────────────────────────────┘
  ❌ Нет Auth    ❌ Нет CORS    ❌ Связанные данные = null
```

### Целевая архитектура MVP
```
┌────────────────────────────────────────────────────────────────────┐
│                    Blazor WASM Client                              │
│        (Product Catalog, Cart, Orders, Auth Pages)                │
├────────────────────────────────────────────────────────────────────┤
│                       API Layer                                    │
│   ┌──────────┐ ┌─────────┐ ┌──────────┐ ┌───────────┐            │
│   │AuthCtrl  │ │Products │ │OrderCtrl │ │UsersCtrl  │  + others  │
│   │          │ │ Ctrl    │ │          │ │           │            │
│   └──────────┘ └─────────┘ └──────────┘ └───────────┘            │
├────────────────────────────────────────────────────────────────────┤
│  Middleware Pipeline: ErrorHandling → Auth → CORS → Antiforgery  │
├────────────────────────────────────────────────────────────────────┤
│                    Application Layer                               │
│   ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌────────────┐          │
│   │AuthSvc   │ │ProductSvc│ │OrderSvc  │ │CreditSvc   │ + others│
│   └──────────┘ └──────────┘ └──────────┘ └────────────┘          │
├────────────────────────────────────────────────────────────────────┤
│                    Domain / Mapping Layer                          │
│   ┌──────────────────────────────────────────────────┐            │
│   │  ProductMapping (with Include / projections)      │           │
│   │  CategoryMapping, OrderMapping, UserMapping       │           │
│   └──────────────────────────────────────────────────┘            │
├────────────────────────────────────────────────────────────────────┤
│                    Infrastructure Layer                             │
│   ┌──────────────┐ ┌──────────┐ ┌───────────────┐                │
│   │UnitOfWork(DI)│ │Repos (9+)│ │TokenService   │                │
│   └──────────────┘ └──────────┘ └───────────────┘                │
├────────────────────────────────────────────────────────────────────┤
│                    Data Layer                                      │
│   ┌──────────────────────────────────────────────────┐            │
│   │  SnipeItContext (READ-ONLY для Snipe-IT таблиц)  │           │
│   │  + WebShop-specific таблицы (Credits, Tokens)     │           │
│   └──────────────────────────────────────────────────┘            │
├────────────────────────────────────────────────────────────────────┤
│               MariaDB (Snipe-IT + WebShop schema)                 │
│           ⬇️ SSH tunnel (dev) / direct (prod on VM)               │
└────────────────────────────────────────────────────────────────────┘
```

### Слои и ответственности

| Слой | Ответственность | Файлы |
|------|----------------|-------|
| **API Layer** | HTTP routing, валидация, авторизация, формат ответов | Controllers/, Middleware/ |
| **Application Layer** | Бизнес-логика, оркестрация, маппинг | Services/, Mapping/ |
| **Domain Layer** | Модели, исключения, правила | Models/, Exceptions/, Validators/ |
| **Infrastructure Layer** | Доступ к данным, внешние сервисы | Repositories/, SnipeItContext |
| **Shared Layer** | DTOs для клиент-серверного обмена | WebShopMercantec.Shared/DTOs/ |

---

## 3. Структура папок проекта

### Текущая структура (что есть)
```
WebShopMercantec/
├── WebShopMercantec/                    # Server project
│   ├── Configuration/                   # ✅ Есть (Serilog, но не используется)
│   ├── Controllers/                     # ✅ 6 контроллеров
│   ├── Exceptions/                      # ✅ 6 custom exceptions
│   ├── Extensions/                      # ✅ ValidationExtensions
│   ├── Mapping/                         # ✅ 6 маппингов (с TODO-заглушками)
│   ├── Middleware/                       # ✅ ErrorHandlingMiddleware
│   ├── Models/                          # ✅ 54 scaffolded модели + SnipeItContext
│   ├── Repositories/                    # ✅ Generic + Specific (9 repos)
│   ├── Services/                        # ✅ 6 сервисов + интерфейсы
│   └── Validators/                      # ✅ 7 валидаторов
├── WebShopMercantec.Client/             # Blazor WASM (пустой)
└── WebShopMercantec.Shared/             # DTOs
    └── DTOs/                            # ✅ 14 DTO классов
```

### Целевая структура MVP (что добавить)
```
WebShopMercantec/
├── WebShopMercantec/
│   ├── Configuration/
│   │   ├── SerilogConfiguration.cs      # ✅ Обновить — использовать из Program.cs
│   │   ├── JwtConfiguration.cs          # ❌ СОЗДАТЬ
│   │   └── CorsConfiguration.cs         # ❌ СОЗДАТЬ
│   ├── Controllers/
│   │   ├── AuthController.cs            # ❌ СОЗДАТЬ
│   │   ├── UsersController.cs           # ❌ СОЗДАТЬ
│   │   ├── OrdersController.cs          # ❌ СОЗДАТЬ
│   │   ├── CreditsController.cs         # ❌ СОЗДАТЬ
│   │   └── ... (существующие 6)
│   ├── Services/
│   │   ├── IAuthService.cs              # ❌ СОЗДАТЬ
│   │   ├── AuthService.cs               # ❌ СОЗДАТЬ
│   │   ├── ITokenService.cs             # ❌ СОЗДАТЬ
│   │   ├── TokenService.cs              # ❌ СОЗДАТЬ
│   │   ├── IUserService.cs              # ❌ СОЗДАТЬ
│   │   ├── UserService.cs               # ❌ СОЗДАТЬ
│   │   ├── IOrderService.cs             # ❌ СОЗДАТЬ
│   │   ├── OrderService.cs              # ❌ СОЗДАТЬ
│   │   ├── ICreditService.cs            # ❌ СОЗДАТЬ
│   │   ├── CreditService.cs             # ❌ СОЗДАТЬ
│   │   └── ... (существующие 6)
│   ├── Repositories/Specific/
│   │   ├── ICreditTransactionRepository # ❌ СОЗДАТЬ (если отдельная таблица)
│   │   └── ... (существующие)
│   ├── Mapping/
│   │   ├── UserMapping.cs               # ❌ СОЗДАТЬ
│   │   ├── OrderMapping.cs              # ❌ СОЗДАТЬ
│   │   └── ... (существующие — ИСПРАВИТЬ TODO)
│   └── Models/
│       ├── CreditTransaction.cs         # ❌ СОЗДАТЬ (новая таблица)
│       └── ... (существующие scaffolded)
├── WebShopMercantec.Shared/DTOs/        # ✅ Уже есть нужные DTO
└── WebShopMercantec.Tests/              # ❌ СОЗДАТЬ (xUnit)
    ├── Unit/
    │   ├── Services/
    │   └── Mapping/
    └── Integration/
```

---

## 4. Поток запроса (Request Lifecycle)

```
Клиент (Blazor WASM / Swagger / Postman)
    │
    ▼
HTTP Request: GET /api/products/5
    │
    ▼
┌──────────────────────────────────────┐
│ 1. ErrorHandlingMiddleware           │  ← Ловит все exceptions
│    try { await _next(context); }     │
│    catch → JSON error response       │
├──────────────────────────────────────┤
│ 2. Authentication Middleware (JWT)   │  ← ❌ НЕ СУЩЕСТВУЕТ ЕЩЁ
│    Проверяет Bearer token            │
├──────────────────────────────────────┤
│ 3. CORS Middleware                   │  ← ❌ НЕ НАСТРОЕН
├──────────────────────────────────────┤
│ 4. Routing → ProductsController      │
│    [HttpGet("{id}")] GetProduct(5)   │
├──────────────────────────────────────┤
│ 5. ProductService.GetProductByIdAsync│
│    → Бизнес-логика                   │
│    → Вызывает UnitOfWork.Products    │
├──────────────────────────────────────┤
│ 6. ProductRepository.GetByIdAsync    │
│    → EF Core query к MariaDB        │
│    → Через SSH туннель (dev)         │
├──────────────────────────────────────┤
│ 7. ProductMapping.MapAssetToDto      │
│    → Entity → DTO трансформация      │
│    → ❌ Связанные поля = null        │
├──────────────────────────────────────┤
│ 8. Controller → Ok(productDto)       │
│    → 200 JSON response               │
└──────────────────────────────────────┘
    │
    ▼
HTTP Response: 200 OK + JSON
```

---

## 5. Конкретные исправления с кодом

### 5.1 Убрать чувствительные файлы из Git

Добавить в `.gitignore`:
```gitignore
# Sensitive files
code.txt
dump.txt
appsettings.*.json
!appsettings.json
```

Команды:
```bash
git rm --cached WebShopMercantec/WebShopMercantec/code.txt
git rm --cached dump.txt
git commit -m "Remove sensitive files from tracking"
```

### 5.2 Перенести ConnectionString в User Secrets (для разработки)

```bash
cd WebShopMercantec/WebShopMercantec
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=127.0.0.1;Port=3307;Database=snipeit;User=snipeit;Password=НОВЫЙ_ПАРОЛЬ;"
```

Production `appsettings.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "" 
  },
  "Jwt": {
    "Issuer": "WebShopMercantec",
    "Audience": "WebShopMercantec.Client",
    "ExpiryInMinutes": 60,
    "RefreshTokenExpiryInDays": 7
  }
}
```

> ❗ В production `ConnectionStrings:DefaultConnection` задаётся через переменную окружения:
> `ConnectionStrings__DefaultConnection=Server=...`

### 5.3 Исправить ProductRepository — загрузить связанные данные

**Текущий код (ProductRepository.cs):**
```csharp
// Просто берёт Asset без связей → маппинг получает null
return await _dbSet.AsNoTracking().Where(...).ToListAsync();
```

**Исправленный код:**
```csharp
public async Task<IEnumerable<Asset>> GetAvailableProductsAsync()
{
    // Загружаем Asset → Model → Category/Manufacturer + StatusLabel + Location
    return await _context.Assets
        .AsNoTracking()
        .Where(a => 
            (a.StatusId == 1 || a.StatusId == 2) &&
            (a.Archived == false || a.Archived == null) &&
            a.DeletedAt == null &&
            a.Requestable == 1 &&
            a.AssignedTo == null)
        .Select(a => new 
        {
            Asset = a,
            Model = _context.Models.FirstOrDefault(m => m.Id == a.ModelId),
            StatusLabel = _context.StatusLabels.FirstOrDefault(s => s.Id == a.StatusId),
            Location = _context.Locations.FirstOrDefault(l => l.Id == a.LocationId)
        })
        .ToListAsync();
}
```

> **Почему `.Select` а не `.Include`?**
> Scaffolded модели НЕ имеют navigation properties (нет `public virtual Model Model { get; set; }`).
> Снайп-IT scaffolding не генерирует связи. Поэтому используем projection через `Select` + sub-queries,
> или добавляем navigation properties вручную в partial classes.

### 5.4 Исправить ProductMapping — убрать TODO-заглушки

**Подход: расширить метод маппинга для приёма связанных данных:**
```csharp
public static class ProductMapping
{
    public static ProductDto MapAssetToDto(
        Asset asset, 
        Model? model = null, 
        Category? category = null,
        Manufacturer? manufacturer = null,
        StatusLabel? statusLabel = null,
        Location? location = null)
    {
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
            CategoryName = category?.Name ?? "Unknown",
            ManufacturerId = manufacturer != null ? (int?)manufacturer.Id : null,
            ManufacturerName = manufacturer?.Name,
            LocationId = asset.LocationId,
            LocationName = location?.Name,
            Notes = asset.Notes,
            PurchaseCost = asset.PurchaseCost,
            Price = asset.PurchaseCost ?? 0m,
            OrderNumber = asset.OrderNumber,
            IsAvailable = asset.StatusId is 1 or 2 
                          && asset.AssignedTo == null 
                          && asset.Requestable == 1,
            Requestable = asset.Requestable == 1
        };
    }
}
```

### 5.5 Исправить N+1 в CategoryService

```csharp
public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
{
    _logger.LogInformation("Getting all categories");
    
    // ОДИН запрос вместо N+1:
    var categories = await _unitOfWork.Categories.GetAllActiveCategoriesAsync();
    var categoryCounts = await _unitOfWork.Categories.GetAllItemsCountsAsync();
    // Возвращает Dictionary<uint, int> — categoryId → count
    
    var categoryDtos = categories.Select(c => 
        CategoryMapping.MapToDto(c, categoryCounts.GetValueOrDefault(c.Id, 0))
    ).ToList();
    
    _logger.LogInformation("Found {Count} categories", categoryDtos.Count);
    return categoryDtos;
}
```

Новый метод в ICategoryRepository:
```csharp
Task<Dictionary<uint, int>> GetAllItemsCountsAsync();
```

Реализация:
```csharp
public async Task<Dictionary<uint, int>> GetAllItemsCountsAsync()
{
    return await _context.Models
        .Where(m => m.DeletedAt == null && m.CategoryId.HasValue)
        .Join(_context.Assets.Where(a => a.DeletedAt == null),
              m => m.Id, a => (uint?)a.ModelId,
              (m, a) => new { CategoryId = (uint)m.CategoryId!.Value })
        .GroupBy(x => x.CategoryId)
        .ToDictionaryAsync(g => g.Key, g => g.Count());
}
```

### 5.6 Исправить UnitOfWork — использовать DI

```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly SnipeItContext _context;
    private IDbContextTransaction? _transaction;

    // Инжектим через конструктор вместо lazy new:
    public IUserRepository Users { get; }
    public IProductRepository Products { get; }
    public IOrderRepository Orders { get; }
    public IAccessoryRepository Accessories { get; }
    public ICategoryRepository Categories { get; }
    public IManufacturerRepository Manufacturers { get; }
    public ISupplierRepository Suppliers { get; }
    public ILocationRepository Locations { get; }
    public IStatusLabelRepository StatusLabels { get; }

    public UnitOfWork(
        SnipeItContext context,
        IUserRepository users,
        IProductRepository products,
        IOrderRepository orders,
        IAccessoryRepository accessories,
        ICategoryRepository categories,
        IManufacturerRepository manufacturers,
        ISupplierRepository suppliers,
        ILocationRepository locations,
        IStatusLabelRepository statusLabels)
    {
        _context = context;
        Users = users;
        Products = products;
        Orders = orders;
        Accessories = accessories;
        Categories = categories;
        Manufacturers = manufacturers;
        Suppliers = suppliers;
        Locations = locations;
        StatusLabels = statusLabels;
    }
    
    // SaveChangesAsync, BeginTransaction и т.д. — без изменений
}
```

### 5.7 Удалить test-assets endpoint

Удалить из `Program.cs`:
```csharp
// УДАЛИТЬ:
app.MapGet("/test-assets", async (SnipeItContext db) => ...);
```

### 5.8 Исправить async anti-pattern

```csharp
// БЫЛО (ProductService.cs):
private Task<ProductDto> MapAssetToDtoAsync(Asset asset)
{
    return Task.FromResult(ProductMapping.MapAssetToDto(asset));
}

// СТАЛО — сделать sync, убрать async обёртку:
private ProductDto MapAssetToDto(Asset asset)
{
    return ProductMapping.MapAssetToDto(asset);
}

// И обновить вызовы:
products.Add(MapAssetToDto(asset));  // вместо await MapAssetToDtoAsync(asset)
```

### 5.9 Добавить CORS

В `Program.cs`:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("WebShopPolicy", policy =>
    {
        policy
            .WithOrigins("https://localhost:5001", "http://localhost:5000") 
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ...

app.UseCors("WebShopPolicy"); // После UseRouting, до UseAuthorization
```

### 5.10 Исправить appsettings.Development.json (невалидный JSON)

Убрать комментарии (JSON не поддерживает `//`):
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=127.0.0.1;Port=3307;Database=snipeit;User=snipeit;Password=Merc2024!;"
  }
}
```
> Или лучше: **удалить пароль и использовать User Secrets** (см. 5.2)

---

## 6. Поэтапный MVP Roadmap

### Шаг 1: Безопасность и подключение БД (1-2 дня)

| Задача | Приоритет | Файлы |
|--------|-----------|-------|
| Добавить `code.txt`, `dump.txt` в `.gitignore` | 🔴 P0 | `.gitignore` |
| Перенести ConnectionString в User Secrets | 🔴 P0 | Program.cs, appsettings |
| Добавить CORS | 🔴 P0 | Program.cs |
| Удалить `/test-assets` endpoint | 🔴 P0 | Program.cs |
| Исправить `appsettings.Development.json` | 🟡 P1 | appsettings.Development.json |
| Добавить production ConnectionString placeholder в appsettings.json | 🟡 P1 | appsettings.json |

### Шаг 2: Исправить существующие endpoint-ы (2-3 дня)

| Задача | Приоритет | Файлы |
|--------|-----------|-------|
| Загрузить связанные данные в ProductRepository (Select/projection) | 🔴 P0 | ProductRepository.cs |
| Исправить ProductMapping — убрать TODO-null | 🔴 P0 | ProductMapping.cs |
| Исправить N+1 в CategoryService | 🟡 P1 | CategoryService.cs, CategoryRepository.cs |
| Рефакторить UnitOfWork → DI injection | 🟡 P1 | UnitOfWork.cs, Program.cs |
| Исправить async anti-pattern в ProductService | 🟢 P2 | ProductService.cs |

### Шаг 3: Аутентификация (3-5 дней)

```
Добавить NuGet пакеты:
  - Microsoft.AspNetCore.Authentication.JwtBearer
  - BCrypt.Net-Next (для проверки Snipe-IT bcrypt паролей)
  - System.IdentityModel.Tokens.Jwt

Создать:
  1. JwtSettings в appsettings.json
  2. ITokenService + TokenService (генерация JWT)
  3. IAuthService + AuthService (login, register, password verify)
  4. AuthController:
     - POST /api/auth/login    → проверить bcrypt, выдать JWT
     - POST /api/auth/register → создать User
     - GET  /api/auth/me       → текущий пользователь
     - POST /api/auth/refresh  → обновить JWT
  5. Настроить JWT middleware в Program.cs
  6. Добавить [Authorize] ко всем контроллерам
  7. Добавить [AllowAnonymous] к каталогу (GET Products)
```

> ⚠️ **Важно:** Snipe-IT хэширует пароли через Laravel bcrypt с префиксом `$2y$`.
> `BCrypt.Net-Next` поддерживает верификацию, но нужно заменить `$2y$` → `$2a$` перед проверкой.

### Шаг 4: Логирование (уже сделано, доработать)

| Задача | Статус |
|--------|--------|
| Console logging | ✅ Готово |
| File logging с ротацией | ✅ Готово |
| Структурированные логи | ✅ Готово |
| Использовать `SerilogConfiguration.cs` вместо inline | ❌ Рефакторить |
| Добавить Request/Response logging middleware | ❌ Создать |
| Добавить логирование auth событий | ❌ После auth |

### Шаг 5: Error Handling (уже сделано ✅)

Всё на месте:
- ✅ ErrorHandlingMiddleware
- ✅ 6 custom exceptions
- ✅ JSON error responses
- ✅ StackTrace только в dev

### Шаг 6: User Service + Credit System (3-4 дня)

```
Создать:
  1. IUserService + UserService
     - GetUserByIdAsync → UserDto
     - GetCurrentUserAsync → UserDto (из JWT claims)
     - GetUserCreditsAsync → decimal
     - UpdateProfileAsync
  
  2. ICreditService + CreditService
     - GetBalanceAsync(userId)
     - AddCreditsAsync(userId, amount, reason)
     - DeductCreditsAsync(userId, amount, reason)
     - GetTransactionHistoryAsync(userId)
  
  3. UsersController
     - GET /api/users/me
     - GET /api/users/{id} [Admin]
     - GET /api/users/{id}/credits
  
  4. CreditsController [Admin]
     - POST /api/credits/user/{userId}/add
     - GET  /api/credits/transactions

  5. Модель CreditTransaction (новая таблица в БД):
     CREATE TABLE credit_transactions (
       id INT AUTO_INCREMENT PRIMARY KEY,
       user_id INT NOT NULL,
       amount DECIMAL(10,2) NOT NULL,
       type ENUM('credit','debit','refund') NOT NULL,
       reason VARCHAR(500),
       related_order_id INT NULL,
       created_at DATETIME DEFAULT CURRENT_TIMESTAMP
     );
     
     ALTER TABLE users ADD COLUMN available_credits DECIMAL(10,2) DEFAULT 0;
```

### Шаг 7: Order Service (3-5 дней)

```
Создать:
  1. IOrderService + OrderService
     Workflow:
     a) Пользователь выбирает товар → CreateOrderAsync:
        - Проверить: продукт доступен? (IsAvailableForCheckoutAsync)
        - Проверить: достаточно кредитов? (HasSufficientCreditsAsync)
        - Начать транзакцию (UnitOfWork.BeginTransactionAsync)
        - Списать кредиты (CreditService.DeductCreditsAsync)
        - Создать CheckoutRequest (status = Pending)
        - Commit транзакцию
     
     b) Админ одобряет → ApproveOrderAsync:
        - Обновить Asset: AssignedTo = userId, StatusId = 3 (Deployed)
        - Обновить CheckoutRequest: FulfilledAt = DateTime.UtcNow
     
     c) Админ отклоняет → DeclineOrderAsync:
        - Вернуть кредиты (CreditService.AddCreditsAsync, type = refund)
        - Обновить CheckoutRequest: CanceledAt = DateTime.UtcNow
     
     d) Пользователь отменяет → CancelOrderAsync:
        - Вернуть кредиты
        - Обновить CheckoutRequest

  2. OrdersController
     - POST /api/orders              → Создать заказ
     - GET  /api/orders/my           → Мои заказы
     - GET  /api/orders/{id}         → Детали заказа
     - POST /api/orders/{id}/cancel  → Отменить
     - GET  /api/orders              → Все заказы [Admin]
     - POST /api/orders/{id}/approve → Одобрить [Admin]
     - POST /api/orders/{id}/decline → Отклонить [Admin]
```

### Шаг 8: Тестирование (2-3 дня)

```
Создать WebShopMercantec.Tests (xUnit):
  
  NuGet пакеты:
  - xUnit
  - Moq
  - FluentAssertions
  - Microsoft.AspNetCore.Mvc.Testing (для integration tests)

  Unit тесты:
  - ProductServiceTests (маппинг, пагинация, NotFoundException)
  - CategoryServiceTests (N+1 fix verification)
  - OrderServiceTests (полный workflow, кредиты, откат)
  - AuthServiceTests (bcrypt verification)
  
  Integration тесты:
  - ProductsControllerTests (GET endpoints)
  - AuthControllerTests (login flow)
```

---

## 7. Security рекомендации

### Немедленно

| # | Действие | Почему |
|---|----------|--------|
| 1 | **Сменить пароль** `Merc2024!` на сервере | Утёк в Git |
| 2 | Очистить Git историю (BFG/filter-repo) | Пароль в коммитах |
| 3 | Перенести секреты в User Secrets / env vars | Нельзя хранить в коде |
| 4 | Добавить JWT аутентификацию | Все endpoint-ы открыты |
| 5 | Добавить `[Authorize]` | CRUD без защиты |

### До production

| # | Действие | Почему |
|---|----------|--------|
| 6 | HTTPS обязательно | Данные в открытом виде |
| 7 | Rate limiting на /api/auth/login | Brute force атаки |
| 8 | Input sanitization | XSS через Notes/Name поля |
| 9 | CORS — указать конкретные origins | Не `*` |
| 10 | Security headers (X-Content-Type-Options, etc.) | Стандарт безопасности |
| 11 | Не возвращать StackTrace в production | Утечка внутренней информации |

### Пример production `appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning",
      "WebShopMercantec": "Information"
    }
  },
  "AllowedHosts": "your-domain.dk;localhost",
  "ConnectionStrings": {
    "DefaultConnection": ""
  },
  "Jwt": {
    "Issuer": "WebShopMercantec",
    "Audience": "WebShopMercantec.Client",
    "ExpiryInMinutes": 60,
    "RefreshTokenExpiryInDays": 7,
    "Key": ""
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Warning"
    }
  }
}
```

> ❗ `Jwt:Key` и `ConnectionStrings:DefaultConnection` задаются через:
> - Docker: `-e ConnectionStrings__DefaultConnection="..."`
> - Linux: `export Jwt__Key="your-secret-key-min-32-chars"`
> - Azure: App Configuration / Key Vault

---

## 8. DevOps рекомендации

### Dockerfile

```dockerfile
# === STAGE 1: Build ===
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем csproj файлы и restore
COPY WebShopMercantec.sln ./
COPY WebShopMercantec/WebShopMercantec/WebShopMercantec.csproj WebShopMercantec/WebShopMercantec/
COPY WebShopMercantec/WebShopMercantec.Client/WebShopMercantec.Client.csproj WebShopMercantec/WebShopMercantec.Client/
COPY WebShopMercantec.Shared/WebShopMercantec.Shared.csproj WebShopMercantec.Shared/
RUN dotnet restore

# Копируем всё остальное и build
COPY . .
RUN dotnet publish WebShopMercantec/WebShopMercantec/WebShopMercantec.csproj \
    -c Release -o /app/publish --no-restore

# === STAGE 2: Runtime ===
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Создаём директорию для логов
RUN mkdir -p /app/logs

COPY --from=build /app/publish .

# Не запускаем от root
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "WebShopMercantec.dll"]
```

### docker-compose.yml (для развёртывания на VM)

```yaml
version: '3.8'

services:
  webshop:
    build: .
    ports:
      - "8060:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=snipeit-db-1;Port=3306;Database=snipeit;User=snipeit;Password=${DB_PASSWORD};
      - Jwt__Key=${JWT_SECRET_KEY}
      - Jwt__Issuer=WebShopMercantec
      - Jwt__Audience=WebShopMercantec.Client
    volumes:
      - webshop-logs:/app/logs
    networks:
      - snipeit_default  # Подключаемся к сети Snipe-IT контейнеров
    restart: unless-stopped
    depends_on:
      - snipeit-db  # Или через external network

volumes:
  webshop-logs:

networks:
  snipeit_default:
    external: true  # Используем существующую сеть Snipe-IT
```

### Убрать пароль из кода — чеклист

| Среда | Метод | Как |
|-------|-------|-----|
| **Локальная разработка** | .NET User Secrets | `dotnet user-secrets set "ConnectionStrings:DefaultConnection" "..."` |
| **Docker** | Environment Variables | `-e ConnectionStrings__DefaultConnection="..."` или `.env` файл |
| **CI/CD** | Pipeline Secrets | GitHub Secrets / GitLab CI Variables |
| **Production** | Environment Variables | Заданы в docker-compose или systemd service |

### Подготовка к деплою — чеклист

- [ ] Создать `Dockerfile` (multi-stage)
- [ ] Создать `docker-compose.yml`
- [ ] Создать `.env.example` (без паролей, с плейсхолдерами)
- [ ] Убедиться что `ASPNETCORE_ENVIRONMENT=Production`
- [ ] Проверить что Swagger отключён в Production
- [ ] Проверить что StackTrace не возвращается в Production
- [ ] Health check endpoint (`/health`)
- [ ] Логи пишутся в volume
- [ ] HTTPS настроен (или через reverse proxy / nginx)

---

## 9. Архитектурные риски и как их избежать

### 🔴 Риск 1: Snipe-IT обновляется — всё ломается

**Проблема:** 54 модели scaffolded из БД Snipe-IT. При обновлении Snipe-IT (новые столбцы, изменённые типы) — модели устаревают.

**Решение:**
1. **НЕ запускать `dotnet ef dbcontext scaffold` бездумно** — каждый re-scaffold перетрёт ручные изменения
2. Использовать **partial classes** для расширения scaffolded моделей
3. Для WebShop-specific данных (кредиты, заказы) — **отдельные таблицы** в той же БД, с отдельным `DbContext` или в том же `SnipeItContext` но с явным `modelBuilder.Entity<CreditTransaction>()`
4. Перед обновлением Snipe-IT: сравнить схему (diff), обновить модели точечно

### 🔴 Риск 2: SSH туннель = single point of failure

**Проблема:** Разработка и (вероятно) CI зависят от SSH туннеля к VM.

**Решение:**
1. В **production**: запускать WebShop контейнер на той же VM в Docker network с Snipe-IT — без туннеля
2. В **dev**: документировать процедуру SSH туннеля, добавить retry logic в EF Core
3. Добавить **health check** для подключения к БД
4. Рассмотреть **VPN** вместо SSH туннеля для команды

### 🟡 Риск 3: Модификация Snipe-IT схемы

**Проблема:** Добавление `available_credits` в таблицу `users` Snipe-IT может сломать Snipe-IT.

**Решение:**
1. **Лучший подход:** Создать отдельную таблицу `webshop_user_credits` с FK на `users.id`
2. Не трогать Snipe-IT таблицы напрямую
3. Все WebShop-specific данные в отдельных таблицах с префиксом `webshop_`

### 🟡 Риск 4: Нет тестов — регрессии

**Проблема:** 0 тестов. Любое изменение может сломать работающий код.

**Решение:**
1. Начать с unit тестов для сервисов (mock UnitOfWork через Moq)
2. Добавить integration тесты для критичных flow (orders, auth)
3. Минимум: тесты для OrderService (деньги!) и AuthService (безопасность!)

### 🟡 Риск 5: SnipeItContext — 2873 строки

**Проблема:** Огромный auto-generated контекст. Невозможно поддерживать вручную.

**Решение:**
1. Не редактировать `SnipeItContext.cs` напрямую — это scaffolded файл
2. Использовать `partial class SnipeItContext` для WebShop-specific конфигурации
3. Создать файл `SnipeItContext.WebShop.cs`:
```csharp
public partial class SnipeItContext
{
    public DbSet<CreditTransaction> CreditTransactions { get; set; }
    public DbSet<WebShopOrder> WebShopOrders { get; set; }
    
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CreditTransaction>(entity =>
        {
            entity.ToTable("webshop_credit_transactions");
            // ... конфигурация
        });
    }
}
```

### 🟢 Риск 6: Blazor WASM vs Server

**Проблема:** Включены оба режима (InteractiveServer + InteractiveWebAssembly), но клиентских страниц нет. WASM увеличивает размер bundle и сложность auth.

**Рекомендация для MVP:**
1. **Оставить InteractiveServer** для начала (проще, не нужен API auth для рендеринга)
2. API endpoints (`/api/*`) — для REST клиентов (Swagger, mobile, SPA)
3. После MVP: решить нужен ли WASM или хватит Server-side Blazor

---

## 📋 Итоговый checklist: что сделать чтобы MVP не пришлось переписывать

- [x] ~~Repository + UnitOfWork pattern~~ (уже сделано)
- [x] ~~DTOs в отдельном проекте~~ (уже сделано)
- [x] ~~Error handling middleware~~ (уже сделано)
- [x] ~~Structured logging~~ (уже сделано)
- [ ] **Загрузить связанные данные в маппинг** (убрать TODO-null)
- [ ] **Исправить N+1 запросы** (CategoryService)
- [ ] **Добавить аутентификацию** (JWT)
- [ ] **Добавить CORS**
- [ ] **Убрать секреты из кода** (User Secrets)
- [ ] **Создать Credit таблицу** (отдельная от Snipe-IT)
- [ ] **Создать Order service** (с транзакциями)
- [ ] **Добавить тесты** (хотя бы для OrderService и AuthService)
- [ ] **Создать Dockerfile** (multi-stage)
- [ ] **Добавить Health Checks**

---

> **Общая оценка времени до рабочего MVP:**  
> 🕐 **15-25 рабочих дней** (1 разработчик)  
> Включая: auth, orders, credits, fix mapping, тесты, Dockerfile

> **Приоритеты:**  
> 1. Безопасность (день 1-2)  
> 2. Фикс маппинга и N+1 (день 3-4)  
> 3. Auth (день 5-9)  
> 4. Credits + Orders (день 10-17)  
> 5. Тесты + DevOps (день 18-25)

