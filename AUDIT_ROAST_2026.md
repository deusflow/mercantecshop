# 🔥 ЖЁСТКАЯ ПРОЖАРКА WebShopMercantec — Полный технический аудит

> **Дата:** 2 марта 2026  
> **Аудитор:** AI Code Reviewer  
> **Цель:** Безжалостный разбор текущего состояния + production-ready план  
> **Стек:** .NET 9 / Blazor WASM / ASP.NET Web API / EF Core 9 / Pomelo MySQL / MariaDB (Snipe-IT)  
> **Вердикт:** Проект на **~18% готовности**. Каркас собран, но MVP нет. Ниже — почему, и что с этим делать.

---

## 📑 Содержание

1. [ПРОЖАРКА: Что сейчас сломано](#1-прожарка-что-сейчас-сломано)
2. [Архитектура: текущая vs целевая](#2-архитектура-текущая-vs-целевая)
3. [Структура проекта по слоям](#3-структура-проекта-по-слоям)
4. [Поток запроса (Request Lifecycle)](#4-поток-запроса-request-lifecycle)
5. [Конкретные примеры кода: что исправить и как](#5-конкретные-примеры-кода-что-исправить-и-как)
6. [Поэтапный MVP Roadmap (реальный)](#6-поэтапный-mvp-roadmap)
7. [Security рекомендации](#7-security-рекомендации)
8. [DevOps рекомендации](#8-devops-рекомендации)
9. [Архитектурные риски + Как не переписывать MVP](#9-архитектурные-риски--как-не-переписывать-mvp)

---

## 1. ПРОЖАРКА: Что сейчас сломано

### 🟢 Что реально хорошо (не обесценивай)

| Компонент | Файлы | Оценка |
|-----------|-------|--------|
| Generic Repository `IRepository<T>` + `Repository<T>` | 240 строк, 17 методов | ✅ Качественно. `AsNoTracking`, пагинация, предикаты — всё на месте |
| 9 Specific Repositories (Product, User, Order, Accessory, Category, Manufacturer, Supplier, Location, StatusLabel) | 18 файлов | ✅ Хорошая декомпозиция |
| Unit of Work | IUnitOfWork + UnitOfWork | ✅ Транзакции, координация репозиториев |
| ErrorHandlingMiddleware | 208 строк | ✅ Профессионально: 6 custom exceptions → правильные HTTP status codes, JSON format, StackTrace только в dev |
| 6 Custom Exceptions | NotFoundException, BadRequest, Unauthorized, Forbidden, InsufficientCredits, ProductNotAvailable | ✅ Продуманная иерархия |
| FluentValidation | 7 валидаторов | ✅ Чистая валидация, правильно зарегистрированы |
| Serilog | Console + File с ротацией | ✅ Работает |
| Swagger + XML docs | Program.cs | ✅ API документация из коробки |
| DTOs в Shared проекте | 14 DTO классов | ✅ Правильное разделение клиент/сервер |
| Dockerfile | Multi-stage, non-root user | ✅ Production-ready |
| docker-compose.yml | Подключение к сети Snipe-IT, env vars, healthcheck | ✅ Грамотно |
| .gitignore | code.txt, dump.txt, .env, appsettings.Development.json | ✅ Чувствительные файлы исключены |

**Итого: инфраструктурный каркас СОБРАН. Это не «начало проекта» — это 18% готового MVP с правильным фундаментом.**

---

### 🔴 КРИТИЧЕСКИЕ ПРОБЛЕМЫ (если не починить — проект не стартует)

#### ❌ ПРОБЛЕМА 1: ВСЕ endpoint-ы ПУБЛИЧНЫЕ

**Где:** Все 6 контроллеров

```
GET  /api/products         → Открыт ✅ (каталог — норма)
POST /api/categories       → Открыт ❌ (любой может создать категорию)
PUT  /api/categories/1     → Открыт ❌ (любой может обновить)
DELETE /api/categories/1   → Открыт ❌ (ЛЮБОЙ может УДАЛИТЬ)
```

**Факт:** Прямо сейчас, если запустить проект и дать URL коллеге — он может удалить все категории, поставщиков, и производителей через Swagger. Просто нажать «Try it out» → «Execute».

**Нет:**
- JWT аутентификации
- `[Authorize]` атрибутов  
- Role-based access  
- Никакой защиты вообще

**Почему критично:** Это магазин. Без auth нельзя понять КТО покупает. Без auth нет Orders, нет Credits, нет User Profile. Без auth — это не магазин, это публичный каталог с возможностью порушить данные.

---

#### ❌ ПРОБЛЕМА 2: Маппинг возвращает NULL и "Unknown" вместо реальных данных

**Где:** `Mapping/ProductMapping.cs` (строки 27-41)

```csharp
// ТЕКУЩИЙ КОД — ЧТО ВИДИТ КЛИЕНТ:
{
    "name": "Dell Latitude 5520",
    "modelName": null,            // ← NULL
    "categoryName": "Unknown",    // ← ЗАГЛУШКА
    "manufacturerName": null,     // ← NULL
    "statusLabel": "Status 2",   // ← МУСОР (число в строке)
    "locationName": null,         // ← NULL
    "supplierName": null,         // ← NULL
    "companyName": null           // ← NULL
}
```

**Почему так:** Scaffolded модели из Snipe-IT **НЕ имеют navigation properties**. EF Core не генерирует `public virtual Model Model { get; set; }` при scaffold из чужой БД. Поэтому `Asset` — это просто flat table с FK-полями (`ModelId`, `StatusId`, `LocationId`), но без `Include()`.

Маппинг честно написал `// TODO: получить из Model` — и оставил null.

**Результат:** API работает, но возвращает бесполезные данные. Фронт показывает "Unknown" вместо "Laptops", null вместо "Dell".

---

#### ❌ ПРОБЛЕМА 3: ConnectionString в Production = пусто → crash

**Где:** `appsettings.json` (строка 10)

```json
"ConnectionStrings": {
    "DefaultConnection": ""  // ← ПУСТАЯ СТРОКА
}
```

**Что происходит при `ASPNETCORE_ENVIRONMENT=Production`:**
1. ASP.NET загружает `appsettings.json` (base)
2. НЕ загружает `appsettings.Development.json` (он для dev)
3. ConnectionString = `""` 
4. `ServerVersion.AutoDetect("")` → **crash при старте**

**В docker-compose.yml** это решено через env var `ConnectionStrings__DefaultConnection`, но если запустить `dotnet run` без Docker в Production — мгновенный crash.

---

#### ❌ ПРОБЛЕМА 4: Пароль `Merc2024!` в `appsettings.Development.json`

**Где:** `appsettings.Development.json` (строка 11)

```json
"DefaultConnection": "Server=127.0.0.1;Port=3307;Database=snipeit;User=snipeit;Password=Merc2024!;"
```

**Статус:** `.gitignore` содержит `appsettings.Development.json` — то есть файл НЕ коммитится. **Это правильно.** Но:

1. Если файл УЖЕ попал в историю Git раньше — пароль утёк
2. `code.txt` и `dump.txt` тоже в `.gitignore`, но если были закоммичены — Git помнит
3. Пароль `Merc2024!` используется для: SSH root, MySQL root, MySQL snipeit, Snipe-IT superadmin, и MAIL — **ОДИН ПАРОЛЬ НА ВСЁ** 💀

---

### 🟡 СЕРЬЁЗНЫЕ ПРОБЛЕМЫ (исправить до MVP)

#### ⚠️ ПРОБЛЕМА 5: N+1 запросов в CategoryService

**Где:** `Services/CategoryService.cs` (строки 32-39)

```csharp
var categories = await _unitOfWork.Categories.GetAllActiveCategoriesAsync();
foreach (var category in categories)
{
    var itemsCount = await _unitOfWork.Categories.GetItemsCountAsync(category.Id);
    // ↑ Отдельный SQL запрос для КАЖДОЙ категории
    categoryDtos.Add(CategoryMapping.MapToDto(category, itemsCount));
}
```

**Математика:** 50 категорий = 1 запрос (GetAll) + 50 запросов (GetItemsCount) = **51 SQL запрос вместо 1-2**.

Это не «ну и ладно, работает». При каждом открытии страницы каталога — 51 запрос к MariaDB. Через SSH туннель. С latency. Пользователь увидит Loading... 3-5 секунд.

---

#### ⚠️ ПРОБЛЕМА 6: UnitOfWork обходит DI контейнер

**Где:** `Repositories/UnitOfWork.cs` (строки 42-50, 55-62, и т.д.)

```csharp
public IUserRepository Users
{
    get
    {
        _users ??= new UserRepository(_context);  // ← new вместо DI
        return _users;
    }
}
```

**Что не так:**
1. `new UserRepository(_context)` — создаёт объект вручную, обходя DI
2. Если завтра `UserRepository` получит новую зависимость (ILogger, ICacheService) — конструктор UnitOfWork ничего об этом не узнает
3. Mock-тестирование: нельзя подменить репозиторий в тестах через DI
4. Нарушение Dependency Inversion Principle

**Но!** Работает? Работает. Критично? Нет. Просто технический долг, который укусит при росте.

---

#### ⚠️ ПРОБЛЕМА 7: `uint` vs `int` — мина замедленного действия

**Где:** Модели используют `uint Id` (MySQL UNSIGNED INT), DTOs и контроллеры — `int`.

```csharp
// Models/Asset.cs:
public uint Id { get; set; }

// DTOs/ProductDto.cs:
public int Id { get; set; }

// Services/ProductService.cs:
var asset = await _unitOfWork.Products.GetByIdAsync((uint)id);  // ← Cast

// Mapping/ProductMapping.cs:
Id = (int)asset.Id,  // ← Cast
```

**Риск:** `uint.MaxValue = 4,294,967,295`, `int.MaxValue = 2,147,483,647`. При ID > 2.1 млрд — `OverflowException`. Но реально? В Snipe-IT школы — вряд ли будет 2 млрд assets. **Не критично, но грязно.**

---

#### ⚠️ ПРОБЛЕМА 8: SerilogConfiguration.cs существует, но не используется

**Где:** `Configuration/SerilogConfiguration.cs` — красивый, документированный класс конфигурации Serilog.

**Факт:** В `Program.cs` Serilog настроен inline (строки 22-32). `SerilogConfiguration.cs` НЕ вызывается нигде. Мёртвый код.

---

#### ⚠️ ПРОБЛЕМА 9: Blazor WASM включён, но пуст

**Где:** `WebShopMercantec.Client/`

```
Client/Pages/Counter.razor  ← Дефолтный template (не используется)
Client/Program.cs           ← Пустой bootstrap (без HttpClient, без auth)
```

В `Program.cs` сервера:
```csharp
.AddInteractiveServerComponents()
.AddInteractiveWebAssemblyComponents()
```

Оба режима включены. Server-side Blazor pages есть (Home, Login, ProductDetails, UserProfile, UserManagement). WASM — пустышка.

**Проблема:** Двойной render mode = двойная сложность. WASM требует отдельный HttpClient, отдельную auth (JWT в local storage), отдельный bundling. Server-side Blazor получает данные напрямую через DI — проще.

---

#### ⚠️ ПРОБЛЕМА 10: Нет Health Check endpoint

**Где:** `docker-compose.yml` (строка 17)

```yaml
healthcheck:
  test: ["CMD-SHELL", "curl -f http://localhost:8080/health || exit 1"]
```

Docker compose проверяет `/health`, но **такого endpoint не существует** в приложении. Healthcheck будет вечно в статусе `unhealthy`.

---

### 🟢 МЕЛОЧИ (не срочно, но неприятно)

| # | Проблема | Где | Влияние |
|---|----------|-----|---------|
| 11 | `Jwt.Key` = `""` в prod appsettings — crash при инициализации JWT | appsettings.json | Crash в production |
| 12 | `package.json` с `sass` — не интегрирован в build pipeline | package.json | SCSS не компилируется автоматически |
| 13 | `BACKEND_ROADMAP_RU.md`, `PROJECT_GUIDE_RU.md` — не проверял актуальность | *.md | Могут вводить в заблуждение |
| 14 | `WebShopMercantec.sln.DotSettings.user` — IDE-специфичный файл в repo | root | Загрязняет repo |
| 15 | `EnrichedAccessory.cs`, `EnrichedAsset.cs` — скорее всего не используются | Models/ | Мёртвый код |
| 16 | `ProductDto` раздут (30+ полей) — клиенту не нужны все для каталога | DTOs/ProductDto.cs | Лишний payload |

---

## 2. Архитектура: текущая vs целевая

### Текущее состояние

```
┌─────────────────────────────────────────────────────────────┐
│                    Blazor Server Pages                       │
│  Home.razor, Login.razor, ProductDetails.razor...           │
│  (UI есть, но НЕ подключён к API — рендерит заглушки)      │
├─────────────────────────────────────────────────────────────┤
│                     6 Controllers                            │
│  Products, Categories, Manufacturers, Suppliers,            │
│  Locations, StatusLabels                                    │
│  ❌ Нет AuthController, UsersController, OrdersController   │
│  ❌ Нет [Authorize]                                         │
├─────────────────────────────────────────────────────────────┤
│                      6 Services                              │
│  ProductService, CategoryService, ManufacturerService,      │
│  SupplierService, LocationService, StatusLabelService       │
│  ❌ Нет AuthService, UserService, OrderService, CreditSvc   │
├─────────────────────────────────────────────────────────────┤
│                   Static Mapping Layer                       │
│  6 Mapping классов — ProductMapping, CategoryMapping...     │
│  ⚠️ Связанные данные = null / "Unknown" / "Status 2"       │
├─────────────────────────────────────────────────────────────┤
│              Repository + Unit of Work                       │
│  IRepository<T> (generic) + 9 specific repos                │
│  UnitOfWork (lazy new, не DI)                               │
├─────────────────────────────────────────────────────────────┤
│           SnipeItContext (2873 строки, scaffolded)           │
│  54 DbSet, OnModelCreating = автосгенерирован               │
│  Нет navigation properties (нет Include)                    │
├─────────────────────────────────────────────────────────────┤
│             MariaDB 11.5.2 (Snipe-IT database)              │
│  SSH tunnel (dev): localhost:3307 → 192.168.115.187:3306    │
│  Docker network (prod): snipeit-db-1:3306                   │
└─────────────────────────────────────────────────────────────┘
```

### Целевая архитектура MVP

```
┌────────────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                               │
│  ┌──────────────────────┐  ┌────────────────────────────────────┐  │
│  │ Blazor Server Pages  │  │          REST API                  │  │
│  │ (SSR, для UI)        │  │   /api/auth, /api/products,       │  │
│  │ Home, Catalog,       │  │   /api/orders, /api/users,        │  │
│  │ ProductDetails,      │  │   /api/credits                    │  │
│  │ Cart, UserProfile    │  │   + Swagger UI (dev only)         │  │
│  └──────────────────────┘  └────────────────────────────────────┘  │
├────────────────────────────────────────────────────────────────────┤
│                    MIDDLEWARE PIPELINE                              │
│  ErrorHandling → JWT Auth → CORS → Antiforgery → Routing         │
├────────────────────────────────────────────────────────────────────┤
│                    APPLICATION LAYER (Services)                     │
│  ┌──────────┐ ┌───────────┐ ┌───────────┐ ┌──────────┐           │
│  │AuthSvc   │ │ProductSvc │ │OrderSvc   │ │CreditSvc │ + 6 more │
│  │- Login   │ │- Catalog  │ │- Create   │ │- Balance │           │
│  │- Register│ │- Search   │ │- Approve  │ │- Add     │           │
│  │- Token   │ │- Paged    │ │- Cancel   │ │- Deduct  │           │
│  └──────────┘ └───────────┘ └───────────┘ └──────────┘           │
├────────────────────────────────────────────────────────────────────┤
│                    DOMAIN LAYER                                     │
│  Models (54 scaffolded + WebShop-specific)                        │
│  Exceptions (6 custom)                                            │
│  Validators (FluentValidation, 7+)                                │
│  Mapping (6 static classes → с загрузкой связей)                  │
├────────────────────────────────────────────────────────────────────┤
│                    INFRASTRUCTURE LAYER                             │
│  ┌──────────────┐  ┌──────────────┐  ┌─────────────────────────┐  │
│  │UnitOfWork(DI)│  │Repos (9+)    │  │TokenService (JWT gen)   │  │
│  │              │  │Generic +     │  │PasswordService (bcrypt) │  │
│  │              │  │Specific      │  │                         │  │
│  └──────────────┘  └──────────────┘  └─────────────────────────┘  │
├────────────────────────────────────────────────────────────────────┤
│                    DATA LAYER                                      │
│  ┌──────────────────────────────────────────────────────────┐     │
│  │ SnipeItContext (scaffolded, READ existing tables)        │     │
│  │ + partial class: WebShop-specific tables                 │     │
│  │   - webshop_credit_transactions                          │     │
│  │   - webshop_refresh_tokens                               │     │
│  └──────────────────────────────────────────────────────────┘     │
├────────────────────────────────────────────────────────────────────┤
│               MariaDB 11.5.2 (Snipe-IT + WebShop tables)          │
│           dev: SSH tunnel → prod: Docker internal network          │
└────────────────────────────────────────────────────────────────────┘
```

### Описание слоёв

#### Domain Layer
- **Что:** Бизнес-правила, модели, исключения, валидация
- **Где:** `Models/`, `Exceptions/`, `Validators/`
- **Зависимости:** Ничего. Domain ни от кого не зависит.
- **Текущее состояние:** 54 scaffolded модели (чужие, из Snipe-IT) + 6 exceptions + 7 validators = **готово на 70%**
- **Доработать:** Добавить WebShop-specific модели (`CreditTransaction`, `RefreshToken`)

#### Application Layer
- **Что:** Бизнес-логика, оркестрация, маппинг DTO ↔ Entity
- **Где:** `Services/`, `Mapping/`
- **Зависимости:** Domain + Infrastructure (через интерфейсы)
- **Текущее состояние:** 6 из ~10 сервисов готовы, маппинг с заглушками = **готово на 40%**
- **Доработать:** AuthService, UserService, OrderService, CreditService. Исправить маппинг.

#### Infrastructure Layer
- **Что:** Доступ к данным, внешние сервисы (JWT, email и т.д.)
- **Где:** `Repositories/`, `SnipeItContext.cs`, будущие TokenService/PasswordService
- **Зависимости:** EF Core, Pomelo, JWT пакеты
- **Текущее состояние:** Repository pattern + UoW = **готово на 80%**
- **Доработать:** DI в UnitOfWork, TokenService, загрузка связей

#### API Layer
- **Что:** HTTP endpoint-ы, routing, авторизация, формат ответов
- **Где:** `Controllers/`, `Middleware/`
- **Зависимости:** Application Layer (через интерфейсы)
- **Текущее состояние:** 6 контроллеров + ErrorMiddleware = **готово на 35%**
- **Доработать:** AuthController, OrdersController, UsersController, CreditsController, `[Authorize]`

---

## 3. Структура проекта по слоям

### Текущая vs Целевая (подробный diff)

```
WebShopMercantec/
├── WebShopMercantec/WebShopMercantec/    # Server project
│   ├── Configuration/
│   │   ├── SerilogConfiguration.cs       # ⚠️ ЕСТЬ, НЕ ИСПОЛЬЗУЕТСЯ → интегрировать
│   │   ├── JwtConfiguration.cs           # ❌ СОЗДАТЬ
│   │   └── ServiceRegistration.cs        # ❌ СОЗДАТЬ (вынести DI из Program.cs)
│   │
│   ├── Controllers/
│   │   ├── ProductsController.cs         # ✅ ЕСТЬ → добавить [Authorize] на POST/PUT/DELETE
│   │   ├── CategoriesController.cs       # ✅ ЕСТЬ → добавить [Authorize] на POST/PUT/DELETE
│   │   ├── ManufacturersController.cs    # ✅ ЕСТЬ
│   │   ├── SuppliersController.cs        # ✅ ЕСТЬ
│   │   ├── LocationsController.cs        # ✅ ЕСТЬ
│   │   ├── StatusLabelsController.cs     # ✅ ЕСТЬ
│   │   ├── AuthController.cs             # ❌ СОЗДАТЬ
│   │   ├── UsersController.cs            # ❌ СОЗДАТЬ
│   │   ├── OrdersController.cs           # ❌ СОЗДАТЬ
│   │   └── CreditsController.cs          # ❌ СОЗДАТЬ
│   │
│   ├── Exceptions/                       # ✅ ВСЁ ГОТОВО (6 классов)
│   │
│   ├── Extensions/
│   │   ├── ValidationExtensions.cs       # ✅ ЕСТЬ
│   │   └── ClaimsPrincipalExtensions.cs  # ❌ СОЗДАТЬ (GetUserId(), GetRole() из JWT)
│   │
│   ├── Mapping/
│   │   ├── ProductMapping.cs             # ⚠️ ЕСТЬ → ИСПРАВИТЬ (убрать TODO-null)
│   │   ├── CategoryMapping.cs            # ✅ ЕСТЬ
│   │   ├── ManufacturerMapping.cs        # ✅ ЕСТЬ
│   │   ├── SupplierMapping.cs            # ✅ ЕСТЬ
│   │   ├── LocationMapping.cs            # ✅ ЕСТЬ
│   │   ├── StatusLabelMapping.cs         # ✅ ЕСТЬ
│   │   ├── UserMapping.cs               # ❌ СОЗДАТЬ
│   │   └── OrderMapping.cs              # ❌ СОЗДАТЬ
│   │
│   ├── Middleware/
│   │   ├── ErrorHandlingMiddleware.cs    # ✅ ГОТОВО
│   │   └── RequestLoggingMiddleware.cs   # ❌ СОЗДАТЬ (опционально)
│   │
│   ├── Models/
│   │   ├── [54 scaffolded models]        # ✅ НЕ ТРОГАТЬ
│   │   ├── SnipeItContext.cs             # ✅ НЕ ТРОГАТЬ (2873 строки)
│   │   ├── SnipeItContext.WebShop.cs     # ❌ СОЗДАТЬ (partial class для WebShop таблиц)
│   │   ├── CreditTransaction.cs         # ❌ СОЗДАТЬ
│   │   └── RefreshToken.cs              # ❌ СОЗДАТЬ
│   │
│   ├── Repositories/
│   │   ├── IRepository.cs               # ✅ ГОТОВО
│   │   ├── Repository.cs                # ✅ ГОТОВО
│   │   ├── IUnitOfWork.cs               # ✅ ГОТОВО → обновить
│   │   ├── UnitOfWork.cs                # ⚠️ ЕСТЬ → переделать на DI
│   │   └── Specific/
│   │       ├── [18 файлов]              # ✅ ГОТОВО
│   │       ├── ICreditTransactionRepo   # ❌ СОЗДАТЬ
│   │       └── CreditTransactionRepo    # ❌ СОЗДАТЬ
│   │
│   ├── Services/
│   │   ├── [12 файлов]                  # ✅ 6 сервисов с интерфейсами
│   │   ├── IAuthService.cs              # ❌ СОЗДАТЬ
│   │   ├── AuthService.cs               # ❌ СОЗДАТЬ
│   │   ├── ITokenService.cs             # ❌ СОЗДАТЬ
│   │   ├── TokenService.cs              # ❌ СОЗДАТЬ
│   │   ├── IUserService.cs              # ❌ СОЗДАТЬ
│   │   ├── UserService.cs               # ❌ СОЗДАТЬ
│   │   ├── IOrderService.cs             # ❌ СОЗДАТЬ
│   │   ├── OrderService.cs              # ❌ СОЗДАТЬ
│   │   ├── ICreditService.cs            # ❌ СОЗДАТЬ
│   │   └── CreditService.cs             # ❌ СОЗДАТЬ
│   │
│   └── Validators/
│       ├── [7 валидаторов]              # ✅ ГОТОВО
│       └── OrderCreateValidator.cs      # ❌ СОЗДАТЬ
│
├── WebShopMercantec.Client/              # Blazor WASM
│   └── Pages/Counter.razor               # ⚠️ МУСОР → решить: нужен WASM или нет?
│
├── WebShopMercantec.Shared/DTOs/         # ✅ 14 DTO — ГОТОВО
│
└── WebShopMercantec.Tests/               # ❌ СОЗДАТЬ
    ├── WebShopMercantec.Tests.csproj
    ├── Unit/
    │   ├── Services/
    │   │   ├── ProductServiceTests.cs
    │   │   ├── OrderServiceTests.cs
    │   │   └── AuthServiceTests.cs
    │   └── Mapping/
    │       └── ProductMappingTests.cs
    └── Integration/
        └── Controllers/
            └── ProductsControllerTests.cs
```

---

## 4. Поток запроса (Request Lifecycle)

### Текущий поток (с пометками что сломано)

```
Клиент (Swagger / Postman / Blazor Pages)
    │
    ▼
HTTP Request: GET /api/products/5
    │
    ▼
┌──────────────────────────────────────────┐
│ 1. ErrorHandlingMiddleware               │
│    try { await _next(context); }         │  ← ✅ Работает
│    catch → JSON error с правильным       │
│            status code                    │
├──────────────────────────────────────────┤
│ 2. Authentication Middleware             │  ← ❌ НЕ СУЩЕСТВУЕТ
│    [должен проверять JWT Bearer token]   │     Любой запрос проходит
├──────────────────────────────────────────┤
│ 3. CORS Middleware                       │  ← ⚠️ Настроен, но для dev
│    app.UseCors("WebShopPolicy")          │     AllowAnyOrigin в dev
├──────────────────────────────────────────┤
│ 4. Routing → ProductsController          │
│    [HttpGet("{id}")] GetProduct(5)       │  ← ✅ Работает
│    Нет [Authorize] → публичный           │
├──────────────────────────────────────────┤
│ 5. ProductService.GetProductByIdAsync(5) │
│    → _unitOfWork.Products.GetByIdAsync   │  ← ✅ Работает
│    → Если null → throw NotFoundException │
│    → Middleware → 404 JSON               │
├──────────────────────────────────────────┤
│ 6. ProductRepository.GetByIdAsync(5)     │
│    → _dbSet.FindAsync(5)                 │  ← ✅ Работает
│    → EF Core → SQL SELECT               │
│    → MariaDB (SSH tunnel / Docker net)   │
│    → Возвращает Asset БЕЗ связей        │  ← ⚠️ Нет Include/Select
├──────────────────────────────────────────┤
│ 7. ProductMapping.MapAssetToDto(asset)   │
│    → ModelName = null                    │  ← ❌ TODO заглушки
│    → CategoryName = "Unknown"            │
│    → ManufacturerName = null             │
│    → StatusLabel = "Status 2"            │
├──────────────────────────────────────────┤
│ 8. Controller → Ok(productDto)           │
│    → 200 JSON response                   │  ← ✅ Формат правильный
│    → НО данные неполные                  │
└──────────────────────────────────────────┘
    │
    ▼
HTTP Response: 200 OK
{
  "id": 5,
  "name": "Dell Latitude 5520",
  "categoryName": "Unknown",     ← бесполезно
  "manufacturerName": null,      ← бесполезно
  "statusLabel": "Status 2",    ← бесполезно
  "price": 0.00                  ← может быть 0 если PurchaseCost = null
}
```

### Целевой поток (как должно быть)

```
Клиент → GET /api/products/5
         Headers: Authorization: Bearer <jwt_token>
    │
    ▼
ErrorHandling → JWT Auth → CORS → Routing
    │
    ▼
ProductsController.GetProduct(5)
  [AllowAnonymous] для GET каталога (или [Authorize] для деталей)
    │
    ▼
ProductService.GetProductByIdAsync(5)
  → UnitOfWork.Products.GetProductWithDetailsAsync(5)
    │
    ▼
ProductRepository — SQL запрос с JOIN:
  SELECT a.*, m.Name AS ModelName, m.ModelNumber,
         c.Name AS CategoryName,
         mfr.Name AS ManufacturerName,
         sl.Name AS StatusName,
         l.Name AS LocationName
  FROM assets a
  LEFT JOIN models m ON m.id = a.model_id
  LEFT JOIN categories c ON c.id = m.category_id
  LEFT JOIN manufacturers mfr ON mfr.id = m.manufacturer_id
  LEFT JOIN status_labels sl ON sl.id = a.status_id
  LEFT JOIN locations l ON l.id = a.location_id
  WHERE a.id = 5 AND a.deleted_at IS NULL
    │
    ▼
ProductMapping.MapAssetToDto(asset, model, category, manufacturer, statusLabel, location)
  → ВСЕ поля заполнены
    │
    ▼
HTTP Response: 200 OK
{
  "id": 5,
  "name": "Dell Latitude 5520",
  "modelName": "Latitude 5520",
  "categoryName": "Laptops",
  "manufacturerName": "Dell",
  "statusLabel": "Ready to Deploy",
  "locationName": "K20 Storage",
  "price": 8500.00,
  "isAvailable": true
}
```

---

## 5. Конкретные примеры кода: что исправить и как

### 5.1 Загрузить связанные данные БЕЗ navigation properties

**Проблема:** Модели scaffolded, нет `virtual Model Model { get; set; }`.

**Решение: Projection через Select (без изменения моделей)**

Добавить DTO-проекцию прямо в репозиторий:

```csharp
// Новый record для internal use (не DTO, а projection result):
public record AssetWithDetails(
    Asset Asset,
    Model? Model,
    Category? Category,
    Manufacturer? Manufacturer,
    StatusLabel? StatusLabel,
    Location? Location,
    Supplier? Supplier);
```

Новый метод в `IProductRepository`:
```csharp
Task<AssetWithDetails?> GetProductWithDetailsAsync(uint id);
Task<IEnumerable<AssetWithDetails>> GetAvailableProductsWithDetailsAsync();
```

Реализация:
```csharp
public async Task<AssetWithDetails?> GetProductWithDetailsAsync(uint id)
{
    return await _context.Assets
        .AsNoTracking()
        .Where(a => a.Id == id && a.DeletedAt == null)
        .Select(a => new AssetWithDetails(
            a,
            _context.Models.FirstOrDefault(m => m.Id == (uint?)a.ModelId),
            _context.Models
                .Where(m => m.Id == (uint?)a.ModelId && m.CategoryId != null)
                .Join(_context.Categories, m => (uint)m.CategoryId!, c => c.Id, (m, c) => c)
                .FirstOrDefault(),
            _context.Models
                .Where(m => m.Id == (uint?)a.ModelId && m.ManufacturerId != null)
                .Join(_context.Manufacturers, m => (uint)m.ManufacturerId!, mfr => mfr.Id, (m, mfr) => mfr)
                .FirstOrDefault(),
            a.StatusId != null 
                ? _context.StatusLabels.FirstOrDefault(s => s.Id == (uint)a.StatusId!) 
                : null,
            a.LocationId != null 
                ? _context.Locations.FirstOrDefault(l => l.Id == (uint)a.LocationId!) 
                : null,
            a.SupplierId != null 
                ? _context.Suppliers.FirstOrDefault(s => s.Id == (uint)a.SupplierId!) 
                : null
        ))
        .FirstOrDefaultAsync();
}
```

Обновлённый маппинг:
```csharp
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
        Notes = asset.Notes,
        PurchaseCost = asset.PurchaseCost,
        Price = asset.PurchaseCost ?? 0m,
        OrderNumber = asset.OrderNumber,
        IsAvailable = asset.StatusId is 1 or 2 
                      && asset.AssignedTo == null 
                      && asset.Requestable == 1
                      && (asset.Archived == false || asset.Archived == null),
        Requestable = asset.Requestable == 1,
        Archived = asset.Archived
    };
}
```

### 5.2 Исправить N+1 в CategoryService

```csharp
// ДОБАВИТЬ в ICategoryRepository:
Task<Dictionary<uint, int>> GetAllItemsCountsBatchAsync();

// РЕАЛИЗАЦИЯ в CategoryRepository:
public async Task<Dictionary<uint, int>> GetAllItemsCountsBatchAsync()
{
    // ОДИН запрос: GROUP BY category_id → count assets
    return await _context.Models
        .AsNoTracking()
        .Where(m => m.DeletedAt == null && m.CategoryId.HasValue)
        .GroupJoin(
            _context.Assets.Where(a => a.DeletedAt == null),
            m => m.Id,
            a => (uint?)a.ModelId,
            (m, assets) => new { CategoryId = (uint)m.CategoryId!.Value, Count = assets.Count() })
        .GroupBy(x => x.CategoryId)
        .Select(g => new { CategoryId = g.Key, Total = g.Sum(x => x.Count) })
        .ToDictionaryAsync(x => x.CategoryId, x => x.Total);
}

// ОБНОВЛЁННЫЙ CategoryService:
public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
{
    var categories = await _unitOfWork.Categories.GetAllActiveCategoriesAsync();
    var counts = await _unitOfWork.Categories.GetAllItemsCountsBatchAsync();
    // 2 запроса вместо 51!
    
    return categories.Select(c => 
        CategoryMapping.MapToDto(c, counts.GetValueOrDefault(c.Id, 0))
    ).ToList();
}
```

### 5.3 Переделать UnitOfWork на DI

```csharp
// UnitOfWork.cs — НОВАЯ ВЕРСИЯ:
public class UnitOfWork : IUnitOfWork
{
    private readonly SnipeItContext _context;
    private IDbContextTransaction? _transaction;

    // Все репозитории через конструктор DI:
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

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    public async Task BeginTransactionAsync() => _transaction = await _context.Database.BeginTransactionAsync();
    public async Task CommitTransactionAsync()
    {
        if (_transaction != null) { await _transaction.CommitAsync(); await _transaction.DisposeAsync(); _transaction = null; }
    }
    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null) { await _transaction.RollbackAsync(); await _transaction.DisposeAsync(); _transaction = null; }
    }
    public void Dispose() { _transaction?.Dispose(); _context.Dispose(); }
}
```

### 5.4 Настроить ConnectionString правильно

**Для dev (User Secrets):**
```bash
cd WebShopMercantec/WebShopMercantec
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Server=127.0.0.1;Port=3307;Database=snipeit;User=snipeit;Password=Merc2024!;"
dotnet user-secrets set "Jwt:Key" "dev-only-secret-key-min-32-characters-long!!"
```

**Для production (env vars через docker-compose):**
```yaml
# docker-compose.yml — уже настроено ✅
environment:
  - ConnectionStrings__DefaultConnection=Server=snipeit-db-1;Port=3306;Database=snipeit;User=snipeit;Password=${DB_PASSWORD};
  - Jwt__Key=${JWT_SECRET_KEY}
```

**В `.env` рядом с docker-compose.yml:**
```env
DB_PASSWORD=НОВЫЙ_СИЛЬНЫЙ_ПАРОЛЬ_ЗДЕСЬ
JWT_SECRET_KEY=your-production-secret-key-at-least-32-characters-long!!
```

**IConfiguration уже подключен правильно в Program.cs:**
```csharp
// Это уже есть и это правильно:
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SnipeItContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
```

ASP.NET Core автоматически мержит: `appsettings.json` → `appsettings.{Environment}.json` → User Secrets → Environment Variables. Приоритет: env vars > User Secrets > appsettings.

### 5.5 Добавить Health Check

```csharp
// Program.cs — добавить:
builder.Services.AddHealthChecks()
    .AddDbContextCheck<SnipeItContext>("database");

// После app.MapControllers():
app.MapHealthChecks("/health");
```

NuGet: `Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore`

### 5.6 Интегрировать SerilogConfiguration.cs

```csharp
// Program.cs — БЫЛО (inline):
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    // ...30 строк...
    .CreateLogger();

// Program.cs — СТАЛО (через конфигурацию):
SerilogConfiguration.ConfigureSerilog();
```

### 5.7 Где нужен DTO и маппинг (AutoMapper vs ручной)

**Текущий подход: ручной маппинг через static классы.**

**Вердикт: ОСТАВИТЬ ручной маппинг.** Причины:
1. 6 маппинг-классов — не так много, чтобы тащить AutoMapper (лишняя зависимость)
2. Projection через Select (как в 5.1) не работает с AutoMapper.ProjectTo для scaffolded моделей без navigation properties
3. Ручной маппинг = полный контроль, explicit, легко дебажить
4. AutoMapper имеет смысл при 20+ маппингов и convention-based подходе

**Где DTO обязателен:**
| Операция | DTO | Зачем |
|----------|-----|-------|
| GET /api/products | `ProductDto` | Не отдавать Entity напрямую (утечка структуры БД) |
| POST /api/auth/login | `LoginDto` | Принимать только нужные поля |
| POST /api/auth/register | `RegisterDto` | Валидация, не принимать Id/Role |
| GET /api/users/me | `UserDto` | Не отдавать password hash |
| POST /api/orders | `OrderCreateDto` | Только ProductId + Quantity |
| GET /api/orders | `OrderDto` | Обогащённые данные (ProductName, UserName) |
| GET /api/credits/transactions | `TransactionDto` | История операций |

**DTO уже созданы в `WebShopMercantec.Shared/DTOs/`** — 14 штук. Они shared между Server и Client (Blazor WASM). Это правильно.

---

## 6. Поэтапный MVP Roadmap

### Фаза 0: Безопасность и гигиена (День 1)

| # | Задача | Файлы | Время |
|---|--------|-------|-------|
| 0.1 | Убедиться что `code.txt`, `dump.txt` не в Git history | `.gitignore` (уже есть) | 15 мин |
| 0.2 | Перенести секреты в User Secrets | CLI | 15 мин |
| 0.3 | Добавить Health Check endpoint `/health` | Program.cs, .csproj | 30 мин |
| 0.4 | Интегрировать `SerilogConfiguration.cs` вместо inline | Program.cs, SerilogConfiguration.cs | 30 мин |
| 0.5 | Удалить `Counter.razor` (дефолтный template мусор) | Client/Pages/ | 5 мин |

### Фаза 1: Починить то что есть (День 2-3)

| # | Задача | Файлы | Время |
|---|--------|-------|-------|
| 1.1 | Загрузить связанные данные в ProductRepository (Select projection) | ProductRepository.cs, IProductRepository.cs | 3-4 часа |
| 1.2 | Обновить ProductMapping — убрать TODO-null, принимать связи | ProductMapping.cs | 1-2 часа |
| 1.3 | Обновить ProductService — передавать связи в маппинг | ProductService.cs | 1-2 часа |
| 1.4 | Исправить N+1 в CategoryService (batch count) | CategoryService.cs, ICategoryRepository.cs, CategoryRepository.cs | 2 часа |
| 1.5 | Рефакторить UnitOfWork → DI | UnitOfWork.cs, Program.cs | 1-2 часа |
| 1.6 | Проверить ВСЕ 6 контроллеров на такие же проблемы | Controllers/, Services/ | 2 часа |

### Фаза 2: Аутентификация (День 4-7)

| # | Задача | Файлы | Время |
|---|--------|-------|-------|
| 2.1 | Установить NuGet: `Microsoft.AspNetCore.Authentication.JwtBearer`, `BCrypt.Net-Next` | .csproj | 10 мин |
| 2.2 | Создать `JwtConfiguration.cs` | Configuration/ | 1 час |
| 2.3 | Создать `ITokenService` + `TokenService` (генерация JWT, refresh tokens) | Services/ | 3-4 часа |
| 2.4 | Создать `IAuthService` + `AuthService` (login, register, verify bcrypt) | Services/ | 4-5 часов |
| 2.5 | Создать `AuthController` (login, register, refresh, me) | Controllers/ | 3-4 часа |
| 2.6 | Настроить JWT middleware в Program.cs | Program.cs | 1-2 часа |
| 2.7 | Добавить `[Authorize]` ко всем контроллерам, `[AllowAnonymous]` к GET каталога | Controllers/ | 1 час |
| 2.8 | Создать `ClaimsPrincipalExtensions.cs` (GetUserId, GetRole) | Extensions/ | 30 мин |
| 2.9 | Тестировать через Swagger (Bearer token) | — | 2 часа |

**⚠️ Нюанс с Snipe-IT bcrypt:**
```csharp
// Snipe-IT (Laravel) использует $2y$ prefix
// BCrypt.Net-Next ожидает $2a$ prefix
// Нужна конвертация:
public bool VerifyPassword(string password, string hashedPassword)
{
    // Laravel bcrypt → .NET bcrypt
    var normalizedHash = hashedPassword.Replace("$2y$", "$2a$");
    return BCrypt.Net.BCrypt.Verify(password, normalizedHash);
}
```

### Фаза 3: WebShop-specific таблицы (День 8-9)

| # | Задача | Файлы | Время |
|---|--------|-------|-------|
| 3.1 | Создать SQL миграцию для `webshop_credit_transactions` | SQL script | 1 час |
| 3.2 | Создать SQL миграцию для `webshop_refresh_tokens` | SQL script | 30 мин |
| 3.3 | Добавить `available_credits` колонку к `users` ИЛИ создать `webshop_user_credits` таблицу | SQL script | 1 час |
| 3.4 | Создать модели: `CreditTransaction.cs`, `RefreshToken.cs` | Models/ | 1-2 часа |
| 3.5 | Создать partial class `SnipeItContext.WebShop.cs` | Models/ | 1-2 часа |
| 3.6 | Создать Repository для CreditTransaction | Repositories/Specific/ | 1-2 часа |

**Рекомендация по таблицам:**

```sql
-- НЕ ТРОГАТЬ таблицу users напрямую!
-- Создать отдельную таблицу для баланса:

CREATE TABLE webshop_user_credits (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT UNSIGNED NOT NULL UNIQUE,
    available_credits DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    total_spent DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE webshop_credit_transactions (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT UNSIGNED NOT NULL,
    amount DECIMAL(10,2) NOT NULL,
    type ENUM('credit', 'debit', 'refund') NOT NULL,
    reason VARCHAR(500),
    balance_before DECIMAL(10,2) NOT NULL,
    balance_after DECIMAL(10,2) NOT NULL,
    related_order_id INT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE webshop_refresh_tokens (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT UNSIGNED NOT NULL,
    token VARCHAR(500) NOT NULL,
    expires_at DATETIME NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    revoked_at DATETIME NULL,
    replaced_by_token VARCHAR(500) NULL,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    INDEX idx_token (token),
    INDEX idx_user_id (user_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
```

### Фаза 4: User Service + Credit System (День 10-13)

| # | Задача | Файлы | Время |
|---|--------|-------|-------|
| 4.1 | Создать `IUserService` + `UserService` | Services/ | 4-5 часов |
| 4.2 | Создать `UserMapping.cs` | Mapping/ | 1-2 часа |
| 4.3 | Создать `UsersController` (GET /api/users/me, GET /api/users/{id}) | Controllers/ | 2-3 часа |
| 4.4 | Создать `ICreditService` + `CreditService` (balance, add, deduct, history) | Services/ | 4-5 часов |
| 4.5 | Создать `CreditsController` | Controllers/ | 2-3 часа |
| 4.6 | Тестировать credit flow через Swagger | — | 2 часа |

### Фаза 5: Order System (День 14-18)

| # | Задача | Файлы | Время |
|---|--------|-------|-------|
| 5.1 | Создать `IOrderService` + `OrderService` | Services/ | 6-8 часов |
| 5.2 | Создать `OrderMapping.cs` | Mapping/ | 1-2 часа |
| 5.3 | Создать `OrdersController` | Controllers/ | 3-4 часа |
| 5.4 | Реализовать полный workflow: Create → Approve/Decline → Cancel | OrderService.cs | 4-5 часов |
| 5.5 | Тестировать Order flow (Create, Check Credits, Approve, Refund) | — | 3 часа |

**Order Workflow:**
```
Пользователь → POST /api/orders { productId: 5, quantity: 1 }
    │
    ├── Проверка: продукт доступен? (IsAvailableForCheckoutAsync)
    │   └── Нет → 409 ProductNotAvailableException
    │
    ├── Проверка: достаточно кредитов? (CreditService.GetBalance >= Price)
    │   └── Нет → 402 InsufficientCreditsException
    │
    ├── BEGIN TRANSACTION
    │   ├── CreditService.DeductCredits(userId, price, "Purchase: Dell Latitude")
    │   ├── Create CheckoutRequest (status = Pending)
    │   └── COMMIT
    │
    └── Return 201 Created { orderId: 42, status: "Pending" }

Админ → POST /api/orders/42/approve
    │
    ├── BEGIN TRANSACTION
    │   ├── Update Asset: AssignedTo = userId, StatusId = 3 (Deployed)
    │   ├── Update CheckoutRequest: FulfilledAt = DateTime.UtcNow
    │   └── COMMIT
    │
    └── Return 200 OK { status: "Fulfilled" }

Админ → POST /api/orders/42/decline { reason: "Out of stock" }
    │
    ├── BEGIN TRANSACTION
    │   ├── CreditService.AddCredits(userId, price, "Refund: Order #42 declined")
    │   ├── Update CheckoutRequest: CanceledAt = DateTime.UtcNow
    │   └── COMMIT
    │
    └── Return 200 OK { status: "Canceled" }
```

### Фаза 6: Тестирование (День 19-22)

| # | Задача | Файлы | Время |
|---|--------|-------|-------|
| 6.1 | Создать `WebShopMercantec.Tests` проект (xUnit) | .csproj, solution | 30 мин |
| 6.2 | Установить NuGet: xUnit, Moq, FluentAssertions | .csproj | 10 мин |
| 6.3 | Unit тесты: ProductService | Tests/Unit/Services/ | 3-4 часа |
| 6.4 | Unit тесты: OrderService (КРИТИЧНО — деньги!) | Tests/Unit/Services/ | 4-5 часов |
| 6.5 | Unit тесты: AuthService (КРИТИЧНО — безопасность!) | Tests/Unit/Services/ | 3-4 часа |
| 6.6 | Unit тесты: CreditService (КРИТИЧНО — баланс!) | Tests/Unit/Services/ | 3-4 часа |
| 6.7 | Integration тесты: ProductsController (GET endpoints) | Tests/Integration/ | 3-4 часа |
| 6.8 | Mapping тесты: ProductMapping с полными данными | Tests/Unit/Mapping/ | 2 часа |

### Фаза 7: Финализация и деплой (День 23-25)

| # | Задача | Файлы | Время |
|---|--------|-------|-------|
| 7.1 | Финальная проверка CORS, HTTPS, security headers | Program.cs | 2 часа |
| 7.2 | Проверить Dockerfile (build + run) | Dockerfile | 1 час |
| 7.3 | Проверить docker-compose на VM | docker-compose.yml | 2 часа |
| 7.4 | Создать `.env.example` с плейсхолдерами | root | 15 мин |
| 7.5 | Smoke тесты на VM (все endpoint-ы) | — | 3 часа |
| 7.6 | Обновить README.md | README.md | 1 час |

---

## 7. Security рекомендации

### 🔴 НЕМЕДЛЕННО (до любой другой работы)

| # | Что | Как | Почему |
|---|-----|-----|--------|
| 1 | Проверить Git history на пароли | `git log --all --oneline -S "Merc2024"` | Если был коммит до .gitignore — пароль утёк |
| 2 | Если утёк — очистить историю | `git filter-repo --invert-paths --path code.txt --path dump.txt` или BFG | Иначе любой с доступом к repo видит пароли |
| 3 | Сменить пароль `Merc2024!` | На сервере: MySQL, SSH, Snipe-IT | Один пароль на 5 сервисов — один leak = всё скомпрометировано |
| 4 | Использовать User Secrets для dev | `dotnet user-secrets set ...` | Не хранить credentials в файлах |

### 🟡 ДО PRODUCTION

| # | Что | Как |
|---|-----|-----|
| 5 | JWT аутентификация | См. Фазу 2 Roadmap |
| 6 | `[Authorize]` на все write endpoints | `[Authorize(Roles = "Admin")]` на POST/PUT/DELETE |
| 7 | `[AllowAnonymous]` на каталог | GET /api/products, GET /api/categories |
| 8 | Rate limiting на `/api/auth/login` | `Microsoft.AspNetCore.RateLimiting` (built-in .NET 8+) |
| 9 | HTTPS | Nginx reverse proxy с Let's Encrypt или HTTPS redirect |
| 10 | Security headers | `X-Content-Type-Options: nosniff`, `X-Frame-Options: DENY` |
| 11 | Input sanitization | HTMLSanitizer для Notes/Name полей (XSS) |
| 12 | CORS — конкретные origins | Не `AllowAnyOrigin()` в production |

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
  "AllowedHosts": "your-domain.dk;192.168.115.187",
  "AllowedOrigins": [
    "https://your-domain.dk",
    "http://192.168.115.187:8060"
  ],
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
      "Default": "Warning",
      "Override": {
        "WebShopMercantec": "Information"
      }
    }
  }
}
```

> ❗ `Jwt:Key` и `ConnectionStrings:DefaultConnection` ВСЕГДА через env vars в production:
> ```
> ConnectionStrings__DefaultConnection=Server=snipeit-db-1;...
> Jwt__Key=your-production-secret-key-minimum-32-characters
> ```

### Пример .env.example

```env
# Database
DB_PASSWORD=CHANGE_ME_STRONG_PASSWORD

# JWT
JWT_SECRET_KEY=CHANGE_ME_AT_LEAST_32_CHARACTERS_LONG

# Email (если понадобится)
# MAIL_PASSWORD=CHANGE_ME
```

### Как убрать пароль из кода — финальная таблица

| Среда | Метод | Файл |
|-------|-------|------|
| **Локальная разработка** | .NET User Secrets | Хранится в `~/.microsoft/usersecrets/` |
| **Docker (dev/staging)** | `.env` файл + docker-compose | `.env` в `.gitignore` |
| **Docker (production)** | Environment Variables напрямую | `docker run -e KEY=VALUE` |
| **CI/CD** | GitHub Secrets / GitLab CI Variables | Настройки репозитория |

---

## 8. DevOps рекомендации

### Dockerfile — оценка текущего

**Текущий Dockerfile: ✅ ХОРОШИЙ**

```
✅ Multi-stage build (build + runtime)
✅ Non-root user (appuser)
✅ Логи в /app/logs
✅ Expose 8080
✅ Production environment
```

**Мелкие улучшения:**

```dockerfile
# Добавить .dockerignore для ускорения build:
# .git, bin, obj, node_modules, *.md, *.txt
```

### docker-compose.yml — оценка текущего

**Текущий docker-compose: ✅ ХОРОШИЙ**

```
✅ Environment variables (не хардкод)
✅ Подключение к snipeit_default network
✅ Volume для логов
✅ Healthcheck
✅ restart: unless-stopped
```

**Что исправить:**

```yaml
# 1. Healthcheck проверяет /health — нужно добавить endpoint (см. 5.5)
# 2. Добавить container_name (уже есть ✅)
# 3. Добавить depends_on для DB (если DB в отдельном compose)
```

### Подготовка к деплою — checklist

```
[ ] .dockerignore создан
[ ] /health endpoint работает
[ ] Swagger отключён в Production (уже ✅ — только в IsDevelopment())
[ ] StackTrace не возвращается в Production (уже ✅ — ErrorHandlingMiddleware проверяет IsDevelopment())
[ ] Логи пишутся в volume (уже ✅ — webshop-logs volume)
[ ] HTTPS настроен (через nginx reverse proxy на VM)
[ ] .env.example создан (без реальных паролей)
[ ] README.md обновлён (как запустить, как настроить)
```

### Процедура деплоя на VM

```bash
# 1. SSH на VM
ssh root@192.168.115.187

# 2. Перейти в директорию проекта
cd /home/administrator/webshop  # или где будет лежать

# 3. Склонировать/обновить код
git pull origin main

# 4. Создать .env (один раз, потом не трогать)
cp .env.example .env
nano .env  # Вставить реальные пароли

# 5. Собрать и запустить
docker-compose up -d --build

# 6. Проверить
docker-compose logs -f webshop
curl http://localhost:8060/health

# 7. Проверить что контейнер видит MariaDB
docker exec webshop-app curl -s http://localhost:8080/health
```

---

## 9. Архитектурные риски + Как не переписывать MVP

### 🔴 РИСК 1: Snipe-IT обновляется → модели устаревают

**Сценарий:** Снайп обновляется, в таблице `assets` появляется новый столбец. Scaffolded модель устаревает.

**Вероятность:** Средняя (Snipe-IT обновляется ~2 раза в год)

**Решение:**
1. **НЕ запускать `dotnet ef dbcontext scaffold` снова** — перетрёт ручные изменения
2. При обновлении: сравнить schema diff, добавить новые поля вручную
3. WebShop-specific таблицы с префиксом `webshop_` — Snipe-IT не трогает
4. Использовать `partial class` для расширений:
   ```csharp
   // Models/Asset.WebShop.cs (partial, не трогается scaffold-ом)
   public partial class Asset
   {
       // Кастомные computed properties для WebShop
       public bool IsAvailableForShop => 
           StatusId is 1 or 2 && AssignedTo == null && Requestable == 1;
   }
   ```

### 🔴 РИСК 2: Модификация Snipe-IT данных → поломка

**Сценарий:** WebShop меняет `Asset.StatusId` при approve → Snipe-IT показывает неправильный статус.

**Вероятность:** Высокая (Order approval ТРЕБУЕТ изменения Asset)

**Решение:**
1. Использовать ТЕ ЖЕ статусы что и Snipe-IT (1=Pending, 2=Ready to Deploy, 3=Deployed)
2. Логировать изменения в `action_logs` (Snipe-IT audit trail)
3. Тестировать в staging: создать заказ через WebShop → проверить в Snipe-IT UI
4. **Не создавать/удалять Assets через WebShop** — только checkout/checkin

### 🟡 РИСК 3: SSH туннель для разработки

**Сценарий:** SSH disconnects → EF Core не может подключиться → exception spam.

**Решение:**
1. В production: Docker network (уже настроено в docker-compose ✅)
2. В dev: Добавить retry policy для EF Core:
   ```csharp
   options.UseMySql(connectionString, serverVersion, 
       mysqlOptions => mysqlOptions.EnableRetryOnFailure(
           maxRetryCount: 5,
           maxRetryDelay: TimeSpan.FromSeconds(10),
           errorNumbersToAdd: null));
   ```
3. Документировать для команды: «Перед запуском — проверь SSH туннель»

### 🟡 РИСК 4: Нет тестов = регрессии

**Решение:** Минимальный набор тестов:
1. `OrderServiceTests` — деньги, транзакции, rollback при ошибке
2. `AuthServiceTests` — bcrypt verify, JWT generation, token validation  
3. `CreditServiceTests` — balance never negative, debit/credit arithmetic
4. `ProductMappingTests` — все поля заполнены когда связи есть

### 🟡 РИСК 5: Blazor WASM усложняет auth

**Сценарий:** WASM требует JWT в localStorage, HttpClient с Bearer header, обработку 401 на клиенте.

**Решение для MVP:** 
- **Убрать WASM**, оставить Server-side Blazor
- Server Blazor получает данные через DI (не HTTP), auth через cookie/session — проще
- API endpoints остаются для Swagger/Postman/будущего SPA
- После MVP: решить нужен ли WASM

### 🟢 РИСК 6: SnipeItContext = 2873 строки

**Решение:** Не трогать. Это auto-generated. Использовать partial class для расширений:
```csharp
// Models/SnipeItContext.WebShop.cs
public partial class SnipeItContext
{
    public DbSet<CreditTransaction> CreditTransactions { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<UserCredits> UserCredits { get; set; } = null!;
}
```

---

## 📊 Итоговая сводка

### Что сделано (18%) — НЕ ВЫКИДЫВАТЬ

| Компонент | Состояние | Действие |
|-----------|-----------|----------|
| Repository Pattern | ✅ Качественно | Оставить, обновить UoW |
| Error Handling | ✅ Качественно | Оставить как есть |
| FluentValidation | ✅ Качественно | Оставить, добавить OrderCreateValidator |
| Serilog | ✅ Работает | Интегрировать SerilogConfiguration.cs |
| Swagger | ✅ Работает | Оставить |
| DTOs (Shared) | ✅ Правильно | Добавить OrderCreateDto |
| Dockerfile | ✅ Production-ready | Добавить .dockerignore |
| docker-compose | ✅ Грамотно | Добавить /health endpoint |

### Что починить (фикс существующего)

| Компонент | Проблема | Приоритет |
|-----------|----------|-----------|
| ProductMapping | TODO-null в 7 полях | 🔴 P0 |
| ProductRepository | Нет загрузки связей | 🔴 P0 |
| CategoryService | N+1 запросов | 🟡 P1 |
| UnitOfWork | `new` вместо DI | 🟡 P1 |
| Health Check | Endpoint не существует | 🟡 P1 |
| SerilogConfiguration | Не используется | 🟢 P2 |

### Что создать (новый код)

| Компонент | Файлы | Приоритет |
|-----------|-------|-----------|
| JWT Auth (TokenService, AuthService, AuthController) | ~8 файлов | 🔴 P0 |
| Credit System (CreditService, CreditsController, SQL tables) | ~6 файлов | 🔴 P0 |
| User Service (UserService, UsersController, UserMapping) | ~5 файлов | 🔴 P0 |
| Order System (OrderService, OrdersController, OrderMapping) | ~5 файлов | 🔴 P0 |
| Tests (xUnit, Moq, FluentAssertions) | ~8-10 файлов | 🟡 P1 |

### Общая оценка времени

```
Фаза 0: Безопасность и гигиена ............ 1 день
Фаза 1: Починить существующее ............. 2 дня
Фаза 2: Аутентификация .................... 4 дня
Фаза 3: WebShop таблицы ................... 2 дня
Фаза 4: Credits + Users ................... 4 дня
Фаза 5: Order System ...................... 5 дней
Фаза 6: Тесты ............................. 4 дня
Фаза 7: DevOps + Deploy ................... 3 дня
─────────────────────────────────────────────────
ИТОГО: ~25 рабочих дней (1 разработчик)
       ~15 рабочих дней (2 разработчика, параллельно)
```

### Если нет 25 дней — приоритетный MVP за 12 дней:

```
День 1:     Безопасность + Health Check
День 2-3:   Починить маппинг + N+1
День 4-7:   Auth (JWT) — без этого нет магазина
День 8-9:   Credit таблицы + CreditService
День 10-11: OrderService + OrderController
День 12:    Smoke тесты + Deploy
─────────────────────────────────────────
Минимум: 12 рабочих дней до рабочего MVP
```

**Что можно отложить:** Полноценные unit тесты, WASM, advanced logging, Rate limiting.  
**Что НЕЛЬЗЯ отложить:** Auth, Credits, Orders, фикс маппинга.

---

> **Заключение:** Каркас собран правильно. Фундамент не надо переписывать. Нужно:
> 1. Починить то что сломано (маппинг, N+1)
> 2. Добавить то чего нет (auth, credits, orders)
> 3. Не трогать scaffolded модели — расширять через partial classes
> 4. Держать WebShop данные в ОТДЕЛЬНЫХ таблицах с префиксом `webshop_`
> 5. Тестировать на VM через Docker — не через SSH туннель


