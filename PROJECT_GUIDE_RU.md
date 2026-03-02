# 📚 Полный учебный гайд по проекту WebShopMercantec

> **Для кого этот гайд:** Для того, кто ВПЕРВЫЕ открывает серьёзный ASP.NET проект и хочет научиться строить такие системы самостоятельно.
>
> **Цель:** После прочтения ты будешь понимать:
> - Зачем нужны все эти слои (Controllers, Services, Repositories)
> - Что такое DI, DTO, Unit of Work, Mapping, Validation
> - Как данные путешествуют от клиента до базы и обратно
> - Почему код написан именно так (не просто "что", а "ЗАЧЕМ")

---

## 📖 Оглавление (Читай по порядку!)

1. [Быстрая карта: с чего начать](#1-быстрая-карта-с-чего-начать)
2. [Что это за проект и какие технологии](#2-что-это-за-проект-и-какие-технологии)
3. [Три проекта в одном решении - зачем?](#3-три-проекта-в-одном-решении---зачем)
4. [Архитектура: слои и их роли](#4-архитектура-слои-и-их-роли)
5. [Поток данных: от HTTP до БД](#5-поток-данных-от-http-до-бд)
6. [Интерфейсы: контракты вместо конкретных классов](#6-интерфейсы-контракты-вместо-конкретных-классов)
7. [DI (Dependency Injection): автоматическая магия](#7-di-dependency-injection-автоматическая-магия)
8. [DTO: безопасная передача данных](#8-dto-безопасная-передача-данных)
9. [Маппинг: переводчик Entity ↔ DTO](#9-маппинг-переводчик-entity--dto)
10. [Enriched данные: решение проблемы N+1](#10-enriched-данные-решение-проблемы-n1)
11. [Repository Pattern: работа с БД](#11-repository-pattern-работа-с-бд)
12. [Unit of Work: дирижёр репозиториев](#12-unit-of-work-дирижёр-репозиториев)
13. [Validation: FluentValidation](#13-validation-fluentvalidation)
14. [Обработка ошибок: Middleware и Exceptions](#14-обработка-ошибок-middleware-и-exceptions)
15. [Entity Framework Core и DbContext](#15-entity-framework-core-и-dbcontext)
16. [Логирование с Serilog](#16-логирование-с-serilog)
17. [Swagger: документация API](#17-swagger-документация-api)
18. [Полный пример: жизнь одного запроса](#18-полный-пример-жизнь-одного-запроса)
19. [Глоссарий терминов](#19-глоссарий-терминов)
20. [Что уже есть в проекте (чеклист)](#20-что-уже-есть-в-проекте-чеклист)
21. [Как начать работать с проектом](#21-как-начать-работать-с-проектом)

---

## 1. Быстрая карта: с чего начать

Если ты новичок, читай файлы в таком порядке:

### Шаг 1: Точка входа
📄 `WebShopMercantec/WebShopMercantec/Program.cs`
- Здесь запускается приложение
- Здесь настраивается база данных
- Здесь регистрируются сервисы (DI)
- Здесь подключаются middleware

### Шаг 2: Посмотри на контроллеры
📄 `Controllers/CategoriesController.cs` — **канонический пример**: полный CRUD + валидация + сервис
📄 `Controllers/ProductsController.cs` — пример GET-only с фильтрами, поиском, пагинацией

Здесь ты увидишь:
- Как принимаются HTTP запросы
- Как вызываются сервисы
- Как работает валидация (POST/PUT в CategoriesController)

### Шаг 3: Посмотри на сервисы
📄 `Services/ProductService.cs` - бизнес-логика продуктов
📄 `Services/CategoryService.cs` - бизнес-логика категорий

Здесь ты увидишь:
- Как применяется бизнес-логика
- Как используется UnitOfWork
- Как работает маппинг

### Шаг 4: Посмотри на репозитории
📄 `Repositories/IRepository.cs` - базовый контракт
📄 `Repositories/Repository.cs` - базовая реализация
📄 `Repositories/Specific/ProductRepository.cs` - специализированный репозиторий

Здесь ты увидишь:
- Как делаются запросы к базе
- Что такое enriched данные
- Как избежать проблемы N+1

### Шаг 5: Посмотри на маппинг
📄 `Mapping/ProductMapping.cs` - преобразование Entity → DTO
📄 `Mapping/CategoryMapping.cs` - маппинг категорий

Здесь ты увидишь:
- Как данные из БД превращаются в JSON для клиента

### Шаг 6: Посмотри на валидацию
📄 `Validators/CategoryDtoValidator.cs` - пример валидатора
📄 `VALIDATION_EXAMPLE.md` - примеры тестирования

### Шаг 7: Посмотри на модели
📄 `Models/EnrichedAsset.cs` - обогащённый продукт
📄 `Models/EnrichedAccessory.cs` - обогащённый аксессуар
📄 `WebShopMercantec.Shared/DTOs/*.cs` - DTO для передачи данных

---

## 2. Что это за проект и какие технологии

### Тип проекта
Это **.NET 9.0** веб-приложение на C#:
- **Сервер:** ASP.NET Core (Web API + Blazor Server)
- **Клиент:** Blazor WebAssembly
- **База данных:** MySQL
- **ORM:** Entity Framework Core

### Ключевые технологии

#### На сервере:
- **ASP.NET Core Controllers** - обработка HTTP запросов
- **Entity Framework Core** - работа с базой данных
- **Serilog** - логирование (консоль + файлы)
- **Swagger** - документация API
- **FluentValidation** - проверка входных данных

#### Архитектурные паттерны:
- **Repository Pattern** - абстракция доступа к данным
- **Unit of Work Pattern** - координация репозиториев и транзакций
- **Dependency Injection** - слабая связанность компонентов
- **DTO Pattern** - безопасная передача данных
- **Service Layer Pattern** - бизнес-логика отдельно от контроллеров

---

## 3. Три проекта в одном решении - зачем?

В файле `WebShopMercantec.sln` есть 3 проекта:

### 3.1 WebShopMercantec (Server) 🖥️

**Что это:** Backend приложения

**Ответственность:**
- Принимает HTTP запросы
- Обрабатывает бизнес-логику
- Ходит в базу данных
- Возвращает JSON (DTO)

**Главные папки:**
- `Controllers/` - API endpoints
- `Services/` - бизнес-логика
- `Repositories/` - доступ к данным
- `Models/` - модели базы данных
- `Mapping/` - преобразование Entity ↔ DTO
- `Validators/` - валидация данных
- `Middleware/` - обработка запросов
- `Exceptions/` - кастомные ошибки

### 3.2 WebShopMercantec.Client (Client) 🎨

**Что это:** Frontend приложения

**Ответственность:**
- Отображает UI пользователю
- Делает запросы к API
- Показывает данные

### 3.3 WebShopMercantec.Shared (Shared Library) 📦

**Что это:** Общая библиотека

**Зачем нужна:**
- DTO используются и на сервере, и на клиенте
- Нет дублирования кода
- Меньше багов ("на сервере одно поле, на клиенте другое")
- IDE автоматически подсказывает типы

**Что в ней:**
- `DTOs/` - классы для передачи данных

**Пример:**
```csharp
// На сервере
public async Task<ActionResult<CategoryDto>> GetById(int id)
{
    var category = await _categoryService.GetCategoryByIdAsync(id);
    return Ok(category); // Возвращает CategoryDto
}

// На клиенте
var response = await Http.GetFromJsonAsync<CategoryDto>($"api/categories/{id}");
// CategoryDto - тот же самый класс!
```

---

## 4. Архитектура: слои и их роли

Проект построен по **слоистой архитектуре** (Layered Architecture):

```
┌─────────────────────────────────────┐
│     CLIENT (HTTP Request)           │
└──────────────┬──────────────────────┘
               ↓
┌─────────────────────────────────────┐
│     CONTROLLER (Принимает запрос)   │  ← Тонкий слой
└──────────────┬──────────────────────┘
               ↓
┌─────────────────────────────────────┐
│     VALIDATOR (Проверяет данные)    │  ← FluentValidation
└──────────────┬──────────────────────┘
               ↓
┌─────────────────────────────────────┐
│     SERVICE (Бизнес-логика)         │  ← Логика приложения
└──────────────┬──────────────────────┘
               ↓
┌─────────────────────────────────────┐
│     MAPPING (Entity ↔ DTO)          │  ← Преобразование
└──────────────┬──────────────────────┘
               ↓
┌─────────────────────────────────────┐
│     UNIT OF WORK (Координатор)      │  ← Управление репозиториями
└──────────────┬──────────────────────┘
               ↓
┌─────────────────────────────────────┐
│     REPOSITORY (Доступ к данным)    │  ← SQL запросы через EF
└──────────────┬──────────────────────┘
               ↓
┌─────────────────────────────────────┐
│     DATABASE (MySQL)                │  ← Хранение данных
└─────────────────────────────────────┘
```

### Почему так много слоёв?

**Простой ответ:** Чтобы код было легче понимать, менять и тестировать.

**Подробный ответ:**

1. **Controller** - "секретарь"
   - Принимает входящие запросы
   - Проверяет валидацию
   - Вызывает нужный сервис
   - Возвращает ответ

2. **Service** - "менеджер"
   - Применяет бизнес-правила ("этот товар нельзя продать, если он занят")
   - Координирует работу нескольких репозиториев
   - Не знает про HTTP

3. **Repository** - "библиотекарь"
   - Знает, как достать данные из БД
   - Не знает про бизнес-правила
   - Может быть заменён (вместо MySQL - PostgreSQL)

---

## 5. Поток данных: от HTTP до БД

Давайте проследим **реальный запрос** шаг за шагом:

### Пример: `GET /api/categories/5`

```
1. КЛИЕНТ
   └─> GET /api/categories/5
   
2. ASP.NET CORE ROUTING
   └─> Находит CategoriesController.GetById(5)
   
3. CONTROLLER
   └─> await _categoryService.GetCategoryByIdAsync(5)
   
4. SERVICE
   ├─> var category = await _unitOfWork.Categories.GetActiveCategoryByIdAsync(5)
   └─> var itemsCount = await _unitOfWork.Categories.GetItemsCountAsync(5)
   
5. REPOSITORY
   ├─> SELECT * FROM categories WHERE id = 5 AND deleted_at IS NULL
   └─> SELECT COUNT(*) FROM assets JOIN models ON ... WHERE category_id = 5
   
6. DATABASE
   └─> Возвращает данные
   
7. MAPPING
   └─> CategoryMapping.MapToDto(category, itemsCount)
   
8. CONTROLLER
   └─> return Ok(categoryDto)
   
9. ASP.NET CORE
   └─> Сериализует CategoryDto в JSON
   
10. КЛИЕНТ
    └─> Получает { "id": 5, "name": "Laptops", "itemsCount": 42, ... }
```

### Что если товар не найден?

```
4. SERVICE
   └─> category == null
   └─> throw new NotFoundException("Category", 5)
   
5. MIDDLEWARE (ErrorHandlingMiddleware)
   └─> Ловит NotFoundException
   └─> return StatusCode(404, { error: "Category with ID 5 not found" })
   
6. КЛИЕНТ
   └─> Получает 404 Not Found
```

---

## 6. Интерфейсы: контракты вместо конкретных классов

### Что такое интерфейс простыми словами?

Интерфейс - это **обещание**: "Я обязуюсь иметь эти методы".

### Пример из жизни:

Представь, что ты нанимаешь водителя. Тебе не важно:
- какая у него машина,
- какого цвета,
- какой год выпуска.

Тебе важно только:
- умеет ли он ВОДИТЬ,
- умеет ли он ПАРКОВАТЬСЯ,
- умеет ли он ЗАПРАВЛЯТЬ машину.

Интерфейс `IDriver` будет выглядеть так:

```csharp
public interface IDriver
{
    void Drive();
    void Park();
    void Refuel();
}
```

### Пример из проекта:

```csharp
// Интерфейс (контракт)
public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    Task<CategoryDto?> GetCategoryByIdAsync(int id);
    Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto);
}

// Реализация (конкретный класс)
public class CategoryService : ICategoryService
{
    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        // Реальная логика
    }
}
```

### Зачем это нужно?

#### 1. Тестирование

Можно создать "фейковый" сервис для тестов:

```csharp
public class FakeCategoryService : ICategoryService
{
    public Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        // Возвращаем тестовые данные без обращения к БД
        return Task.FromResult(new[] { new CategoryDto { Id = 1, Name = "Test" } });
    }
}
```

#### 2. Замена реализации

Сегодня данные в MySQL, завтра можно переключиться на API:

```csharp
public class ApiCategoryService : ICategoryService
{
    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        // Вызываем внешний API вместо БД
        return await _httpClient.GetFromJsonAsync<CategoryDto[]>("https://api.example.com/categories");
    }
}
```

Контроллер **не изменится**, потому что он зависит от `ICategoryService`, а не от конкретного класса!

### Интерфейсы в проекте:

| Интерфейс | Что делает |
|-----------|------------|
| `IRepository<T>` | Базовые CRUD операции для любой Entity |
| `ICategoryRepository` | Специфичные методы для категорий |
| `IProductRepository` | Специфичные методы для продуктов |
| `IAccessoryRepository` | Специфичные методы для аксессуаров |
| `IUnitOfWork` | Доступ ко всем репозиториям + транзакции |
| `ICategoryService` | Бизнес-логика категорий |
| `IProductService` | Бизнес-логика продуктов |

---

## 7. DI (Dependency Injection): автоматическая магия

### Что такое DI простыми словами?

Вместо того, чтобы самому создавать зависимости (`new ...`), ты **просишь систему дать их тебе**.

### Пример БЕЗ DI (плохо):

```csharp
public class CategoriesController
{
    private readonly CategoryService _service;
    
    public CategoriesController()
    {
        var context = new SnipeItContext();
        var categoryRepo = new CategoryRepository(context);
        var productRepo = new ProductRepository(context);
        var unitOfWork = new UnitOfWork(context);
        var logger = new Logger<CategoryService>();
        
        _service = new CategoryService(unitOfWork, logger);
    }
}
```

**Проблемы:**
- Контроллер знает, КАК создать сервис (высокая связанность)
- Тяжело тестировать (нельзя подставить фейк)
- Если изменится конструктор сервиса - нужно менять ВСЕ места, где он создаётся

### Пример С DI (хорошо):

```csharp
public class CategoriesController
{
    private readonly ICategoryService _service;
    
    // Просим систему дать нам сервис
    public CategoriesController(ICategoryService service)
    {
        _service = service;
    }
}
```

**Преимущества:**
- Контроллер НЕ знает, как создать сервис
- Легко тестировать (можем передать FakeCategoryService)
- Если конструктор сервиса изменится - контроллер не изменится

### Как это работает?

В `Program.cs` регистрируем:

```csharp
// Говорим: "Когда кто-то попросит ICategoryService, дай ему CategoryService"
builder.Services.AddScoped<ICategoryService, CategoryService>();
```

Теперь когда ASP.NET создаёт контроллер, он:
1. Видит, что контроллеру нужен `ICategoryService`
2. Смотрит в регистрации
3. Находит, что `ICategoryService` → `CategoryService`
4. Создаёт `CategoryService` (рекурсивно разрешая его зависимости)
5. Передаёт в контроллер

### Lifetimes (время жизни):

| Метод | Когда создаётся | Когда умирает |
|-------|-----------------|---------------|
| `AddTransient` | Каждый раз при запросе | Сразу после использования |
| `AddScoped` | Один раз за HTTP запрос | В конце запроса |
| `AddSingleton` | Один раз при старте приложения | При остановке приложения |

**В проекте используется `AddScoped`** - это идеально для веб-приложений:
- DbContext живёт только в рамках одного запроса
- Не возникает проблем с многопоточностью
- Память освобождается после каждого запроса

---

## 8. DTO: безопасная передача данных

### Что такое DTO?

DTO (Data Transfer Object) - это **объект для передачи данных** между слоями или по сети.

### Зачем нужны DTO, если есть Entity?

#### Проблема 1: Безопасность

Entity из БД может содержать поля, которые нельзя показывать клиенту:

```csharp
// Entity (модель БД)
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }  // ❌ Нельзя отдавать!
    public string Salt { get; set; }          // ❌ Нельзя отдавать!
    public DateTime CreatedAt { get; set; }
}

// DTO (для клиента)
public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    // PasswordHash и Salt НЕ включены! ✅
}
```

#### Проблема 2: Удобство для UI

Клиенту часто нужны данные в другом формате:

```csharp
// Entity
public class Asset
{
    public int Id { get; set; }
    public int? ModelId { get; set; }  // Только ID
}

// DTO
public class ProductDto
{
    public int Id { get; set; }
    public int? ModelId { get; set; }
    public string? ModelName { get; set; }  // ✅ Название для UI
    public string? CategoryName { get; set; }  // ✅ Удобно для отображения
}
```

#### Проблема 3: Стабильность API

Можно менять структуру БД, но API остаётся стабильным:

```csharp
// Изменили БД: разделили Name на FirstName и LastName
public class User
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

// DTO остался прежним - клиенты не сломались!
public class UserDto
{
    public string Name { get; set; }  // = FirstName + " " + LastName
}
```

### DTO в проекте:

Все DTO лежат в `WebShopMercantec.Shared/DTOs/`:

- `CategoryDto` - категория
- `ProductDto` - продукт
- `AccessoryDto` - аксессуар
- `UserDto` - пользователь
- `OrderDto` - заказ
- и другие...

---

## 9. Маппинг: переводчик Entity ↔ DTO

### Что такое маппинг?

Маппинг - это **преобразование** одного типа данных в другой.

Например: `Asset` (Entity) → `ProductDto` (DTO)

### Где в проекте?

Папка `Mapping/`:
- `ProductMapping.cs` - маппинг продуктов и аксессуаров
- `CategoryMapping.cs` - маппинг категорий

### Пример простого маппинга:

```csharp
public static CategoryDto MapToDto(Category category, int itemsCount = 0)
{
    return new CategoryDto
    {
        Id = (int)category.Id,
        Name = category.Name,
        CategoryType = category.CategoryType,
        ItemsCount = itemsCount,
        Image = category.Image,
        CreatedAt = category.CreatedAt,
        UpdatedAt = category.UpdatedAt
    };
}
```

### Зачем выделять маппинг в отдельный слой?

#### 1. Переиспользование

Маппинг используется во многих местах:

```csharp
// В сервисе
var dto = CategoryMapping.MapToDto(category, itemsCount);

// В другом сервисе
var dtos = CategoryMapping.MapToDtos(categories, getItemsCount);
```

#### 2. Чистота кода

Вместо:

```csharp
// Контроллер (плохо)
var dto = new CategoryDto
{
    Id = (int)category.Id,
    Name = category.Name,
    CategoryType = category.CategoryType,
    // ...ещё 10 строк
};
```

Пишем:

```csharp
// Контроллер (хорошо)
var dto = CategoryMapping.MapToDto(category, itemsCount);
```

#### 3. Легко менять

Если нужно изменить формат даты или добавить поле:

```csharp
public static CategoryDto MapToDto(Category category, int itemsCount = 0)
{
    return new CategoryDto
    {
        // ...existing fields...
        CreatedAt = category.CreatedAt?.ToUniversalTime(),  // ✅ Изменили в одном месте!
    };
}
```

Изменение применится везде автоматически!

---

## 10. Enriched данные: решение проблемы N+1

### Что такое проблема N+1?

Это когда ты делаешь **МНОГО запросов к БД** вместо одного.

### Пример плохого кода:

```csharp
// Получаем 100 продуктов
var assets = await _context.Assets.Take(100).ToListAsync();

// Для каждого продукта получаем модель (100 запросов!)
foreach (var asset in assets)
{
    var model = await _context.Models.FindAsync(asset.ModelId);  // ❌ N+1 проблема!
    var manufacturer = await _context.Manufacturers.FindAsync(model.ManufacturerId);  // ❌ Ещё хуже!
}

// Итого: 1 + 100 + 100 = 201 запрос к БД!
```

### Решение: Enriched данные

Вместо того, чтобы делать отдельные запросы, делаем **ОДИН запрос с JOIN'ами**:

```csharp
var enrichedAssets = await (
    from asset in _context.Assets
    // LEFT JOIN с Model
    join model in _context.Models on asset.ModelId equals (int?)model.Id into modelGroup
    from model in modelGroup.DefaultIfEmpty()
    // LEFT JOIN с Category (через Model)
    join category in _context.Categories on model.CategoryId equals (int?)category.Id into categoryGroup
    from category in categoryGroup.DefaultIfEmpty()
    // LEFT JOIN с Manufacturer (через Model)
    join manufacturer in _context.Manufacturers on model.ManufacturerId equals (int?)manufacturer.Id into mfgGroup
    from manufacturer in mfgGroup.DefaultIfEmpty()
    select new EnrichedAsset
    {
        Asset = asset,
        Model = model,
        Category = category,
        Manufacturer = manufacturer
    }
).ToListAsync();

// Итого: 1 запрос вместо 201! 🚀
```

### EnrichedAsset и EnrichedAccessory

Это специальные классы, которые содержат основную Entity + все связанные данные:

```csharp
public class EnrichedAsset
{
    public Asset Asset { get; set; }           // Основные данные
    public Model? Model { get; set; }           // Связанная модель
    public Category? Category { get; set; }     // Связанная категория
    public Manufacturer? Manufacturer { get; set; }  // Связанный производитель
    public StatusLabel? StatusLabel { get; set; }    // Связанный статус
}
```

### Где используется?

В репозиториях есть специальные методы:

```csharp
// Обычный метод (без связей)
Task<IEnumerable<Asset>> GetAvailableProductsAsync();

// Enriched метод (СО СВЯЗЯМИ) - быстрее!
Task<IEnumerable<EnrichedAsset>> GetAvailableProductsEnrichedAsync();
```

### Маппинг enriched данных:

```csharp
// Обычный маппинг (данные неполные)
public static ProductDto MapAssetToDto(Asset asset)
{
    return new ProductDto
    {
        Id = (int)asset.Id,
        Name = asset.Name,
        ModelName = null,  // ❌ Нет данных
        CategoryName = "Unknown",  // ❌ Нет данных
    };
}

// Enriched маппинг (все данные есть!)
public static ProductDto MapEnrichedAssetToDto(EnrichedAsset enriched)
{
    return new ProductDto
    {
        Id = (int)enriched.Asset.Id,
        Name = enriched.Asset.Name,
        ModelName = enriched.Model?.Name,  // ✅ Данные есть!
        CategoryName = enriched.Category?.Name ?? "Unknown",  // ✅ Данные есть!
        ManufacturerName = enriched.Manufacturer?.Name,  // ✅ Данные есть!
    };
}
```

### Результат:

До enriched:
- 201 запрос к БД
- ModelName = null
- CategoryName = "Unknown"

После enriched:
- **1 запрос к БД** ⚡
- ModelName = "MacBook Pro"
- CategoryName = "Laptops"

---

## 11. Repository Pattern: работа с БД

### Что такое Repository?

Repository - это **"библиотекарь"**, который знает, как достать данные из БД.

### Зачем нужен?

#### БЕЗ Repository (плохо):

```csharp
// В контроллере (плохо!)
public async Task<ActionResult> GetCategories()
{
    var categories = await _context.Categories
        .Where(c => c.DeletedAt == null)
        .OrderBy(c => c.Name)
        .ToListAsync();
    
    return Ok(categories);
}
```

**Проблемы:**
- Контроллер знает про SQL
- Дублирование запросов (тот же фильтр в 10 местах)
- Тяжело тестировать
- Нельзя поменять БД

#### С Repository (хорошо):

```csharp
// В репозитории
public async Task<IEnumerable<Category>> GetAllActiveCategoriesAsync()
{
    return await _dbSet
        .Where(c => c.DeletedAt == null)
        .OrderBy(c => c.Name)
        .ToListAsync();
}

// В контроллере (чисто!)
public async Task<ActionResult> GetCategories()
{
    var categories = await _categoryRepo.GetAllActiveCategoriesAsync();
    return Ok(categories);
}
```

### Generic Repository

Базовый репозиторий для любой Entity:

```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(uint id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}
```

**Использование:**

```csharp
IRepository<Category> categoryRepo;
IRepository<Product> productRepo;
// Один интерфейс для всех!
```

### Specific Repository

Специализированный репозиторий с кастомными методами:

```csharp
public interface ICategoryRepository : IRepository<Category>
{
    // Базовые методы из IRepository<Category> уже есть
    
    // Добавляем специфичные методы
    Task<IEnumerable<Category>> GetAllActiveCategoriesAsync();
    Task<IEnumerable<Category>> GetCategoriesByTypeAsync(string type);
    Task<int> GetItemsCountAsync(uint categoryId);
}
```

### Enriched методы в репозитории:

```csharp
// Обычный метод
Task<IEnumerable<Asset>> GetAvailableProductsAsync();

// Enriched метод (со связями) - НАМНОГО быстрее!
Task<IEnumerable<EnrichedAsset>> GetAvailableProductsEnrichedAsync();
```

---

## 12. Unit of Work: дирижёр репозиториев

### Что такое Unit of Work?

Unit of Work - это **"дирижёр"**, который:
- Держит все репозитории в одном месте
- Управляет `SaveChanges()`
- Управляет транзакциями

### Зачем нужен?

#### БЕЗ Unit of Work (плохо):

```csharp
public class OrderService
{
    private readonly IUserRepository _userRepo;
    private readonly IProductRepository _productRepo;
    private readonly IOrderRepository _orderRepo;
    private readonly SnipeItContext _context;
    
    public async Task CreateOrder(...)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        var product = await _productRepo.GetByIdAsync(productId);
        
        var order = new Order { ... };
        await _orderRepo.AddAsync(order);
        
        await _context.SaveChangesAsync();  // Каждый раз вызывать вручную
    }
}
```

#### С Unit of Work (хорошо):

```csharp
public class OrderService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task CreateOrder(...)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        var product = await _unitOfWork.Products.GetByIdAsync(productId);
        
        var order = new Order { ... };
        await _unitOfWork.Orders.AddAsync(order);
        
        await _unitOfWork.SaveChangesAsync();  // Одна точка сохранения!
    }
}
```

### Транзакции: "Всё или ничего"

Когда нужна транзакция:
- Создать заказ + списать деньги + обновить склад
- Если хотя бы одна операция провалится - откатить ВСЁ

```csharp
await _unitOfWork.BeginTransactionAsync();

try
{
    // Операция 1: Создать заказ
    await _unitOfWork.Orders.AddAsync(order);
    await _unitOfWork.SaveChangesAsync();
    
    // Операция 2: Списать кредиты
    user.Credits -= order.TotalPrice;
    _unitOfWork.Users.Update(user);
    await _unitOfWork.SaveChangesAsync();
    
    // Операция 3: Обновить статус товара
    product.StatusId = 3; // "Выдано"
    _unitOfWork.Products.Update(product);
    await _unitOfWork.SaveChangesAsync();
    
    // Всё успешно - фиксируем транзакцию
    await _unitOfWork.CommitTransactionAsync();
}
catch
{
    // Ошибка - откатываем ВСЁ
    await _unitOfWork.RollbackTransactionAsync();
    throw;
}
```

### IUnitOfWork в проекте:

```csharp
public interface IUnitOfWork
{
    // Репозитории
    IUserRepository Users { get; }
    IProductRepository Products { get; }
    IOrderRepository Orders { get; }
    IAccessoryRepository Accessories { get; }
    ICategoryRepository Categories { get; }
    
    // Управление
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
```

---

## 13. Validation: FluentValidation

### Зачем нужна валидация?

Проверять входные данные ОТ КЛИЕНТА:
- Имя категории не пустое?
- Email корректный?
- Цена не отрицательная?

### Пример валидатора:

```csharp
public class CategoryDtoValidator : AbstractValidator<CategoryDto>
{
    public CategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required")
            .MinimumLength(2).WithMessage("Category name must be at least 2 characters")
            .MaximumLength(100).WithMessage("Category name must not exceed 100 characters");

        RuleFor(x => x.CategoryType)
            .Must(BeValidCategoryType)
            .WithMessage("Category type must be one of: asset, accessory, consumable, component, license")
            .When(x => !string.IsNullOrEmpty(x.CategoryType));
    }

    private bool BeValidCategoryType(string? categoryType)
    {
        if (string.IsNullOrEmpty(categoryType))
            return true;

        var validTypes = new[] { "asset", "accessory", "consumable", "component", "license" };
        return validTypes.Contains(categoryType.ToLower());
    }
}
```

### Использование в контроллере:

```csharp
[HttpPost]
public async Task<ActionResult<CategoryDto>> Create([FromBody] CategoryDto categoryDto)
{
    // Валидация
    var validationResult = await _validator.ValidateAsync(categoryDto);
    if (!validationResult.IsValid)
    {
        return BadRequest(validationResult.Errors);
    }

    var created = await _categoryService.CreateCategoryAsync(categoryDto);
    return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
}
```

### Что вернётся клиенту при ошибке:

```json
[
  {
    "propertyName": "Name",
    "errorMessage": "Category name is required",
    "attemptedValue": "",
    "severity": "Error"
  }
]
```

Подробнее смотри: **VALIDATION_EXAMPLE.md**

---

## 14. Обработка ошибок: Middleware и Exceptions

### ErrorHandlingMiddleware

Это **"ловушка для ошибок"**, которая:
- Перехватывает все исключения
- Преобразует их в правильный HTTP ответ
- Логирует ошибки

### Кастомные исключения:

```csharp
throw new NotFoundException("Category", id);  // → 404
throw new BadRequestException("Invalid data");  // → 400
throw new UnauthorizedException("Login required");  // → 401
throw new ForbiddenException("No access");  // → 403
```

### Как это работает:

```csharp
// В сервисе
if (category == null)
    throw new NotFoundException("Category", id);

// Middleware ловит
catch (NotFoundException ex)
{
    return new 
    {
        StatusCode = 404,
        Message = ex.Message  // "Category with ID 5 not found"
    };
}
```

---

## 15. Entity Framework Core и DbContext

### Что такое EF Core?

EF Core (Entity Framework Core) - это **ORM** (Object-Relational Mapping).

**Простыми словами:** Вместо SQL пишешь C#, а EF Core переводит в SQL.

### SnipeItContext

Главный класс для работы с БД:

```csharp
public class SnipeItContext : DbContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<Accessory> Accessories { get; set; }
    // ...
}
```

### Примеры запросов:

```csharp
// LINQ → SQL
var categories = await _context.Categories
    .Where(c => c.DeletedAt == null)
    .OrderBy(c => c.Name)
    .ToListAsync();

// SQL:
// SELECT * FROM categories 
// WHERE deleted_at IS NULL 
// ORDER BY name
```

### AsNoTracking()

Для read-only запросов используй `.AsNoTracking()` - это быстрее:

```csharp
var categories = await _context.Categories
    .AsNoTracking()  // Не отслеживать изменения
    .ToListAsync();
```

---

## 16. Логирование с Serilog

### Зачем нужны логи?

- Понимать, что делает приложение
- Находить ошибки
- Анализировать производительность

### Где логи пишутся:

1. **Консоль** (при разработке)
2. **Файлы** `logs/webshop-*.txt` (rolling daily)

### Использование в коде:

```csharp
public class CategoryService
{
    private readonly ILogger<CategoryService> _logger;
    
    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        _logger.LogInformation("Getting category with ID: {CategoryId}", id);
        
        // ...
        
        if (category == null)
        {
            _logger.LogWarning("Category not found: {CategoryId}", id);
            throw new NotFoundException("Category", id);
        }
        
        return dto;
    }
}
```

---

## 17. Swagger: документация API

В Development режиме доступен Swagger UI:
- URL: `https://localhost:5001/swagger`
- Автоматическая документация всех endpoints
- Можно тестировать запросы прямо в браузере

---

## 18. Полный пример: жизнь одного запроса

### Запрос: `POST /api/categories`

```json
{
  "name": "Laptops",
  "categoryType": "asset"
}
```

### Шаг 1: HTTP Request попадает в ASP.NET Core

```
Browser/Postman
    ↓
POST https://localhost:5001/api/categories
Content-Type: application/json
{...}
```

### Шаг 2: Routing → Controller

```csharp
[Route("api/[controller]")]
public class CategoriesController
{
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CategoryDto categoryDto)
```

### Шаг 3: Валидация

```csharp
var validationResult = await _validator.ValidateAsync(categoryDto);
if (!validationResult.IsValid)
{
    return BadRequest(validationResult.Errors);  // → 400 если не валидно
}
```

### Шаг 4: Вызов сервиса

```csharp
var created = await _categoryService.CreateCategoryAsync(categoryDto);
```

### Шаг 5: Сервис → UnitOfWork → Repository

```csharp
public async Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto)
{
    var category = new Category
    {
        Name = categoryDto.Name,
        CategoryType = categoryDto.CategoryType,
        CreatedAt = DateTime.UtcNow,
    };
    
    await _unitOfWork.Categories.AddAsync(category);
    await _unitOfWork.SaveChangesAsync();
    
    return CategoryMapping.MapToDto(category, 0);
}
```

### Шаг 6: SQL запрос в БД

```sql
INSERT INTO categories (name, category_type, created_at, updated_at)
VALUES ('Laptops', 'asset', '2025-12-19 10:30:00', '2025-12-19 10:30:00');
```

### Шаг 7: Mapping Entity → DTO

```csharp
CategoryMapping.MapToDto(category, 0)
// →
{
    "id": 123,
    "name": "Laptops",
    "categoryType": "asset",
    "itemsCount": 0,
    "createdAt": "2025-12-19T10:30:00Z"
}
```

### Шаг 8: Controller возвращает результат

```csharp
return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
// → HTTP 201 Created
// Location: /api/categories/123
```

### Шаг 9: Client получает ответ

```json
HTTP/1.1 201 Created
Location: /api/categories/123
Content-Type: application/json

{
  "id": 123,
  "name": "Laptops",
  "categoryType": "asset",
  "itemsCount": 0,
  "createdAt": "2025-12-19T10:30:00Z"
}
```

---

## 19. Глоссарий терминов

| Термин | Что это | Пример в проекте |
|--------|---------|------------------|
| **Controller** | Принимает HTTP запросы | `CategoriesController` |
| **Service** | Бизнес-логика | `CategoryService` |
| **Repository** | Доступ к данным | `CategoryRepository` |
| **Unit of Work** | Координатор репозиториев | `UnitOfWork` |
| **DTO** | Объект для передачи данных | `CategoryDto` |
| **Entity** | Модель таблицы БД | `Category` |
| **Mapping** | Преобразование Entity → DTO | `CategoryMapping` |
| **Enriched** | Entity + все связи (один запрос) | `EnrichedAsset` |
| **DI** | Автоматическое создание зависимостей | `AddScoped<>` в Program.cs |
| **Middleware** | Обработчик в pipeline запросов | `ErrorHandlingMiddleware` |
| **Validation** | Проверка входных данных | `CategoryDtoValidator` |
| **DbContext** | Главный класс EF Core для БД | `SnipeItContext` |
| **LINQ** | Запросы к БД на C# | `.Where().OrderBy()` |
| **AsNoTracking** | Режим "только читать" | Быстрее для GET |
| **Transaction** | "Всё или ничего" | `BeginTransaction/Commit/Rollback` |

---

## 20. Что уже есть в проекте (чеклист)

### ✅ Архитектура

- [x] Layered Architecture (Controllers → Services → Repositories)
- [x] Repository Pattern (Generic + Specific)
- [x] Unit of Work Pattern
- [x] Dependency Injection
- [x] DTO Pattern
- [x] Mapping Layer
- [x] Enriched данные (решение N+1)

### ✅ Контроллеры

- [x] `ProductsController` - CRUD для продуктов
- [x] `CategoriesController` - CRUD для категорий (обновлён 19.12.2025)
- [x] `LocationsController`
- [x] `ManufacturersController`
- [x] `StatusLabelsController`
- [x] `SuppliersController`

### ✅ Сервисы

- [x] `ProductService` - бизнес-логика продуктов
- [x] `CategoryService` - бизнес-логика категорий

### ✅ Репозитории

Generic:
- [x] `IRepository<T>` - базовый интерфейс
- [x] `Repository<T>` - базовая реализация

Specific:
- [x] `ProductRepository` + `IProductRepository`
- [x] `AccessoryRepository` + `IAccessoryRepository`
- [x] `CategoryRepository` + `ICategoryRepository`
- [x] `UserRepository` + `IUserRepository`
- [x] `OrderRepository` + `IOrderRepository`

Unit of Work:
- [x] `UnitOfWork` + `IUnitOfWork`

### ✅ Маппинг

- [x] `ProductMapping` - маппинг продуктов и аксессуаров
- [x] `CategoryMapping` - маппинг категорий
- [x] Enriched маппинг (полные данные)

### ✅ Валидация

- [x] FluentValidation подключён
- [x] `CategoryDtoValidator`
- [x] `ProductDtoValidator`
- [x] `OrderDtoValidator`
- [x] `UserDtoValidator`
- [x] `RegisterDtoValidator`
- [x] `LoginDtoValidator`
- [x] `AccessoryDtoValidator`

### ✅ Обработка ошибок

- [x] `ErrorHandlingMiddleware`
- [x] Кастомные исключения:
  - `NotFoundException` → 404
  - `BadRequestException` → 400
  - `UnauthorizedException` → 401
  - `ForbiddenException` → 403
  - `InsufficientCreditsException`
  - `ProductNotAvailableException`

### ✅ Enriched данные

- [x] `EnrichedAsset` - продукт со всеми связями
- [x] `EnrichedAccessory` - аксессуар со всеми связями
- [x] Методы в репозиториях:
  - `GetAvailableProductsEnrichedAsync()`
  - `GetEnrichedAssetByIdAsync()`
  - `GetProductsPagedEnrichedAsync()`
  - `GetAvailableAccessoriesEnrichedAsync()`
  - `GetEnrichedAccessoryByIdAsync()`
  - `GetAccessoriesPagedEnrichedAsync()`

### ✅ Инфраструктура

- [x] Entity Framework Core + MySQL
- [x] Serilog (логирование)
- [x] Swagger (документация API)
- [x] Dependency Injection настроен

### ✅ Документация

- [x] `PROJECT_GUIDE_RU.md` (этот файл)
- [x] `VALIDATION_EXAMPLE.md` - примеры валидации
- [x] `REFACTORING_SUMMARY.md` - описание рефакторинга
- [x] `ARCHITECTURE_DIAGRAM.md` - визуальная схема
- [x] `BACKEND_ROADMAP_RU.md` - план развития
- [x] `REPOSITORY_PATTERN_EXPLAINED_RU.md` - объяснение паттернов
- [x] `ERROR_HANDLING_GUIDE.md` - обработка ошибок

---

## 21. Как начать работать с проектом

### Шаг 1: Клонировать и открыть

```bash
git clone <repository_url>
cd WebShopMercantec
```

### Шаг 2: Настроить строку подключения

В `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;database=snipeit;user=root;password=yourpassword"
  }
}
```

### Шаг 3: Запустить

```bash
dotnet build
dotnet run --project WebShopMercantec/WebShopMercantec
```

### Шаг 4: Открыть Swagger

```
https://localhost:5001/swagger
```

### Шаг 5: Попробовать API

**GET все категории:**
```
GET https://localhost:5001/api/categories
```

**GET категория по ID:**
```
GET https://localhost:5001/api/categories/1
```

**POST создать категорию:**
```json
POST https://localhost:5001/api/categories
Content-Type: application/json

{
  "name": "Test Category",
  "categoryType": "asset"
}
```

---

## 🎓 Заключение

Теперь ты знаешь:

✅ **Архитектуру:** Layered Architecture + Repository + Unit of Work  
✅ **Слои:** Controller → Validator → Service → Mapping → UnitOfWork → Repository → DB  
✅ **Паттерны:** DI, DTO, Repository, Unit of Work, Enriched Data  
✅ **Технологии:** ASP.NET Core, EF Core, FluentValidation, Serilog  
✅ **Оптимизации:** Enriched данные решают N+1 проблему  
✅ **Валидацию:** FluentValidation для проверки данных  
✅ **Ошибки:** Middleware + кастомные исключения  

**Следующий шаг:** Открой код и попробуй добавить свой контроллер/сервис/репозиторий, следуя тем же паттернам!

📚 Дополнительные материалы:
- `VALIDATION_EXAMPLE.md` - как тестировать валидацию
- `ARCHITECTURE_DIAGRAM.md` - визуальная схема
- `REFACTORING_SUMMARY.md` - что было изменено

**Удачи в обучении! 🚀**

