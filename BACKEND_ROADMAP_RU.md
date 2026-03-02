# 🚀 РОАДМАП BACKEND РАЗРАБОТКИ - WebShopMercantec
## Детальный план для 100% завершения Backend

---

## 📊 ТЕКУЩЕЕ СОСТОЯНИЕ ПРОЕКТА

### ✅ Что уже реализовано:
- ✅ Entity Framework Core + MySQL (Pomelo)
- ✅ Базовая структура проекта (ASP.NET Core Blazor)
- ✅ Модели данных (Asset, User, CheckoutRequest, Accessory и др.)
- ✅ DbContext (SnipeItContext) с полным набором таблиц
- ✅ DTOs для основных сущностей (ProductDto, UserDto, OrderDto и др.)
- ✅ Swagger для документации API
- ✅ Один контроллер: ProductsController (базовая версия)
- ✅ Один сервис: ProductService (базовая версия)

### ❌ Что отсутствует:
- ❌ Аутентификация и авторизация (JWT/Cookie)
- ❌ Система управления кредитами пользователей
- ❌ Полный CRUD для всех сущностей
- ❌ Система заказов (checkout/orders)
- ❌ Middleware для обработки ошибок
- ❌ Валидация данных
- ❌ Логирование
- ❌ Repository Pattern
- ❌ Unit of Work Pattern
- ❌ Пагинация и фильтрация
- ❌ Обработка файлов (изображения)
- ❌ Email сервис (уведомления)
- ❌ Админ панель API

---

## 🎯 ROADMAP - ПОЭТАПНЫЙ ПЛАН РАЗРАБОТКИ

---

## 📦 ФАЗА 1: ИНФРАСТРУКТУРА И АРХИТЕКТУРА (Приоритет: КРИТИЧЕСКИЙ)

### 1.1 Создание Repository Pattern
**Цель:** Абстрагировать работу с БД от бизнес-логики

**Файлы для создания:**
```
WebShopMercantec/Repositories/
├── IRepository.cs                    // Generic интерфейс
├── Repository.cs                     // Generic реализация
├── IUnitOfWork.cs                    // Unit of Work интерфейс
├── UnitOfWork.cs                     // Unit of Work реализация
└── Specific/
    ├── IUserRepository.cs            // Специфичный для User
    ├── UserRepository.cs
    ├── IProductRepository.cs         // Специфичный для Product (Asset)
    ├── ProductRepository.cs
    ├── IOrderRepository.cs           // Для CheckoutRequest
    ├── OrderRepository.cs
    ├── IAccessoryRepository.cs
    └── AccessoryRepository.cs
```

**Детальные шаги:**

1. **Создать IRepository.cs:**
```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<bool> ExistsAsync(int id);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
}
```

2. **Создать Repository.cs** - базовая реализация с EF Core

3. **Создать специфичные репозитории** для каждой сущности с доп. методами

4. **Создать IUnitOfWork.cs:**
```csharp
public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IProductRepository Products { get; }
    IOrderRepository Orders { get; }
    IAccessoryRepository Accessories { get; }
    // ... другие репозитории
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
```

5. **Зарегистрировать в Program.cs:**
```csharp
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
```

**Время выполнения:** 6-8 часов

---

### 1.2 Настройка Authentication & Authorization
**Цель:** JWT токены для API + Cookie для Blazor

**Файлы для создания:**
```
WebShopMercantec/
├── Services/Auth/
│   ├── IAuthService.cs
│   ├── AuthService.cs
│   ├── ITokenService.cs
│   └── TokenService.cs
├── Middleware/
│   ├── JwtMiddleware.cs
│   └── ErrorHandlingMiddleware.cs
└── Controllers/
    └── AuthController.cs
```

**Детальные шаги:**

1. **Установить NuGet пакеты:**
```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet add package BCrypt.Net-Next
```

2. **Добавить в appsettings.json:**
```json
{
  "JwtSettings": {
    "SecretKey": "ВАШ_СУПЕР_СЕКРЕТНЫЙ_КЛЮЧ_МИНИМУМ_32_СИМВОЛА",
    "Issuer": "WebShopMercantec",
    "Audience": "WebShopMercantecUsers",
    "ExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7
  },
  "ConnectionStrings": {
    "DefaultConnection": "ваша_строка_подключения"
  }
}
```

3. **Создать ITokenService.cs:**
```csharp
public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
    int? GetUserIdFromToken(string token);
}
```

4. **Создать TokenService.cs** - генерация JWT токенов

5. **Создать IAuthService.cs:**
```csharp
public interface IAuthService
{
    Task<(bool Success, string? Token, string? RefreshToken, UserDto? User, string? Error)> 
        LoginAsync(LoginDto loginDto);
    
    Task<(bool Success, UserDto? User, string? Error)> 
        RegisterAsync(RegisterDto registerDto);
    
    Task<(bool Success, string? Token, string? RefreshToken, string? Error)> 
        RefreshTokenAsync(string refreshToken);
    
    Task<bool> LogoutAsync(int userId);
    Task<bool> ValidatePasswordAsync(string password, string hashedPassword);
    string HashPassword(string password);
}
```

6. **Создать AuthService.cs** - полная реализация

7. **Создать AuthController.cs:**
```csharp
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    [HttpPost("register")]
    [HttpPost("refresh")]
    [HttpPost("logout")]
    [HttpGet("me")] // Get current user
    [HttpPost("change-password")]
}
```

8. **Настроить Authentication в Program.cs:**
```csharp
// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOrAdmin", policy => policy.RequireRole("User", "Admin"));
});

// В app.Configure:
app.UseAuthentication();
app.UseAuthorization();
```

9. **Создать ErrorHandlingMiddleware.cs** для обработки ошибок

10. **Обновить User модель** - добавить поле для RefreshToken и его expiry

**Время выполнения:** 8-10 часов

---

### 1.3 Система логирования
**Цель:** Структурированное логирование всех операций

**Файлы для создания:**
```
WebShopMercantec/Services/Logging/
├── ILoggerService.cs
└── LoggerService.cs
```

**Детальные шаги:**

1. **Установить Serilog:**
```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Enrichers.Environment
```

2. **Настроить в Program.cs:**
```csharp
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/webshop-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
```

3. **Создать LoggerService** для кастомного логирования в БД (ActionLog таблица)

4. **Использовать во всех сервисах**

**Время выполнения:** 2-3 часа

---

## 📦 ФАЗА 2: ОСНОВНЫЕ СЕРВИСЫ И КОНТРОЛЛЕРЫ (Приоритет: ВЫСОКИЙ)

### 2.1 User Management (Управление пользователями)

**Файлы для создания:**
```
WebShopMercantec/
├── Services/
│   ├── IUserService.cs
│   └── UserService.cs
└── Controllers/
    └── UsersController.cs
```

**Функциональность:**

**IUserService.cs:**
```csharp
public interface IUserService
{
    // CRUD
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<UserDto?> GetUserByUsernameAsync(string username);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<IEnumerable<UserDto>> GetAllUsersAsync(int pageNumber, int pageSize);
    Task<UserDto> CreateUserAsync(RegisterDto registerDto);
    Task<UserDto> UpdateUserAsync(int id, UserDto userDto);
    Task<bool> DeleteUserAsync(int id);
    
    // Credits Management
    Task<decimal> GetUserCreditsAsync(int userId);
    Task<bool> AddCreditsAsync(int userId, decimal amount, string reason);
    Task<bool> DeductCreditsAsync(int userId, decimal amount, string reason);
    Task<IEnumerable<TransactionDto>> GetUserTransactionsAsync(int userId);
    
    // Statistics
    Task<int> GetTotalPurchasesAsync(int userId);
    Task<decimal> GetTotalSpentAsync(int userId);
    
    // Validation
    Task<bool> ValidateUserCreditsAsync(int userId, decimal requiredAmount);
    Task<bool> UserExistsAsync(int userId);
}
```

**UsersController.cs endpoints:**
```csharp
[Route("api/[controller]")]
[ApiController]
[Authorize] // Требует аутентификации
public class UsersController : ControllerBase
{
    [HttpGet]                              // GET /api/users
    [HttpGet("{id}")]                      // GET /api/users/5
    [HttpPost]                             // POST /api/users
    [HttpPut("{id}")]                      // PUT /api/users/5
    [HttpDelete("{id}")]                   // DELETE /api/users/5
    
    // Credits
    [HttpGet("{id}/credits")]              // GET /api/users/5/credits
    [HttpPost("{id}/credits/add")]         // POST /api/users/5/credits/add
    [HttpPost("{id}/credits/deduct")]      // POST /api/users/5/credits/deduct
    [HttpGet("{id}/transactions")]         // GET /api/users/5/transactions
    
    // Statistics
    [HttpGet("{id}/statistics")]           // GET /api/users/5/statistics
}
```

**Время выполнения:** 6-8 часов

---

### 2.2 Credit System (Система кредитов)

**Важно:** Кредиты - это основная валюта магазина!

**Файлы для создания:**
```
WebShopMercantec/
├── Models/
│   └── CreditTransaction.cs (создать новую таблицу!)
├── Services/
│   ├── ICreditService.cs
│   └── CreditService.cs
└── Controllers/
    └── CreditsController.cs
```

**Детальные шаги:**

1. **Создать модель CreditTransaction:**
```csharp
public class CreditTransaction
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Type { get; set; } // Purchase, AdminCredit, Refund, Adjustment
    public decimal Amount { get; set; }
    public decimal BalanceBefore { get; set; }
    public decimal BalanceAfter { get; set; }
    public int? RelatedOrderId { get; set; }
    public string? RelatedItemType { get; set; } // Asset, Accessory
    public int? RelatedItemId { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

2. **Добавить поле Credits в User модель:**
```csharp
public partial class User
{
    // Добавить:
    public decimal AvailableCredits { get; set; } = 0;
}
```

3. **Создать миграцию:**
```bash
dotnet ef migrations add AddCreditSystem
dotnet ef database update
```

4. **Создать ICreditService:**
```csharp
public interface ICreditService
{
    Task<decimal> GetBalanceAsync(int userId);
    Task<bool> AddCreditsAsync(int userId, decimal amount, string type, string? description);
    Task<bool> DeductCreditsAsync(int userId, decimal amount, string type, int? orderId, string? description);
    Task<bool> HasSufficientCreditsAsync(int userId, decimal requiredAmount);
    Task<IEnumerable<TransactionDto>> GetTransactionHistoryAsync(int userId, int pageNumber, int pageSize);
    Task<bool> RefundAsync(int transactionId, string reason);
    Task<CreditStatisticsDto> GetStatisticsAsync(int userId);
}
```

5. **Реализовать CreditService** с транзакциями БД

6. **Создать CreditsController** для админов

**Время выполнения:** 6-8 часов

---

### 2.3 Product Management (расширение)

**Обновить существующий ProductService:**

**Добавить в IProductService:**
```csharp
public interface IProductService
{
    // Существующие методы
    Task<IEnumerable<ProductDto>> GetAvailableProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int id);
    
    // НОВЫЕ методы:
    Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId);
    Task<IEnumerable<ProductDto>> GetProductsByManufacturerAsync(int manufacturerId);
    Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm);
    Task<(IEnumerable<ProductDto> Products, int TotalCount)> 
        GetProductsPagedAsync(int pageNumber, int pageSize, string? category, string? search);
    
    // Accessories
    Task<IEnumerable<AccessoryDto>> GetAvailableAccessoriesAsync();
    Task<AccessoryDto?> GetAccessoryByIdAsync(int id);
    
    // Admin CRUD
    Task<ProductDto> CreateProductAsync(ProductDto productDto);
    Task<ProductDto> UpdateProductAsync(int id, ProductDto productDto);
    Task<bool> DeleteProductAsync(int id);
    
    // Availability check
    Task<bool> IsProductAvailableAsync(int productId);
    Task<int> GetAvailableQuantityAsync(int productId);
}
```

**Обновить ProductsController:**
```csharp
[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    [HttpGet]                                    // GET /api/products?page=1&pageSize=20&category=Laptops&search=Dell
    [HttpGet("{id}")]                            // GET /api/products/5
    [HttpGet("category/{categoryId}")]           // GET /api/products/category/3
    [HttpGet("search")]                          // GET /api/products/search?q=laptop
    
    [HttpPost]                                   // POST /api/products (Admin only)
    [Authorize(Roles = "Admin")]
    
    [HttpPut("{id}")]                            // PUT /api/products/5 (Admin only)
    [Authorize(Roles = "Admin")]
    
    [HttpDelete("{id}")]                         // DELETE /api/products/5 (Admin only)
    [Authorize(Roles = "Admin")]
    
    // Accessories
    [HttpGet("accessories")]                     // GET /api/products/accessories
    [HttpGet("accessories/{id}")]                // GET /api/products/accessories/5
}
```

**Время выполнения:** 4-5 часов

---

### 2.4 Order Management (Система заказов)

**Ключевая функциональность магазина!**

**Файлы для создания:**
```
WebShopMercantec/
├── Services/
│   ├── IOrderService.cs
│   └── OrderService.cs
└── Controllers/
    └── OrdersController.cs
```

**IOrderService:**
```csharp
public interface IOrderService
{
    // User operations
    Task<(bool Success, OrderDto? Order, string? Error)> 
        CreateOrderAsync(int userId, int itemId, string itemType, int quantity);
    
    Task<OrderDto?> GetOrderByIdAsync(int orderId);
    Task<IEnumerable<OrderDto>> GetUserOrdersAsync(int userId);
    Task<bool> CancelOrderAsync(int orderId, int userId);
    
    // Admin operations
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync(string? status, int pageNumber, int pageSize);
    Task<IEnumerable<OrderDto>> GetPendingOrdersAsync();
    Task<bool> ApproveOrderAsync(int orderId, int adminId);
    Task<bool> DeclineOrderAsync(int orderId, int adminId, string reason);
    Task<bool> FulfillOrderAsync(int orderId, int adminId);
    
    // Statistics
    Task<OrderStatisticsDto> GetOrderStatisticsAsync(DateTime? from, DateTime? to);
}
```

**Бизнес-логика OrderService:**

1. **CreateOrderAsync:**
   - Проверить, что товар доступен
   - Проверить, что у пользователя достаточно кредитов
   - Зарезервировать кредиты (deduct)
   - Создать CheckoutRequest со статусом Pending
   - Создать транзакцию кредитов
   - Вернуть OrderDto

2. **ApproveOrderAsync:**
   - Проверить статус (должен быть Pending)
   - Назначить товар пользователю (asset.assigned_to = user_id)
   - Обновить статус товара
   - Установить fulfilled_at
   - Логировать действие

3. **CancelOrderAsync / DeclineOrderAsync:**
   - Вернуть кредиты пользователю (refund)
   - Установить canceled_at
   - Логировать

**OrdersController endpoints:**
```csharp
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrdersController : ControllerBase
{
    // User endpoints
    [HttpPost]                                   // POST /api/orders
    [HttpGet("my-orders")]                       // GET /api/orders/my-orders
    [HttpGet("{id}")]                            // GET /api/orders/5
    [HttpPost("{id}/cancel")]                    // POST /api/orders/5/cancel
    
    // Admin endpoints
    [HttpGet]                                    // GET /api/orders?status=Pending
    [Authorize(Roles = "Admin")]
    
    [HttpGet("pending")]                         // GET /api/orders/pending
    [Authorize(Roles = "Admin")]
    
    [HttpPost("{id}/approve")]                   // POST /api/orders/5/approve
    [Authorize(Roles = "Admin")]
    
    [HttpPost("{id}/decline")]                   // POST /api/orders/5/decline
    [Authorize(Roles = "Admin")]
    
    [HttpGet("statistics")]                      // GET /api/orders/statistics
    [Authorize(Roles = "Admin")]
}
```

**Время выполнения:** 8-10 часов

---

### 2.5 Accessory Management

**Файлы для создания:**
```
WebShopMercantec/
├── Services/
│   ├── IAccessoryService.cs
│   └── AccessoryService.cs
└── Controllers/
    └── AccessoriesController.cs
```

**Функциональность:**
- Аксессуары имеют количество (qty)
- Можно заказать несколько штук
- Checkout через AccessoriesCheckout таблицу

**IAccessoryService:**
```csharp
public interface IAccessoryService
{
    Task<IEnumerable<AccessoryDto>> GetAvailableAccessoriesAsync();
    Task<AccessoryDto?> GetAccessoryByIdAsync(int id);
    Task<bool> CheckoutAccessoryAsync(int accessoryId, int userId, int quantity);
    Task<bool> CheckinAccessoryAsync(int checkoutId);
    Task<IEnumerable<AccessoryDto>> GetUserAccessoriesAsync(int userId);
    
    // Admin
    Task<AccessoryDto> CreateAccessoryAsync(AccessoryDto accessoryDto);
    Task<AccessoryDto> UpdateAccessoryAsync(int id, AccessoryDto accessoryDto);
    Task<bool> DeleteAccessoryAsync(int id);
}
```

**Время выполнения:** 4-5 часов

---

## 📦 ФАЗА 3: ДОПОЛНИТЕЛЬНЫЕ СЕРВИСЫ (Приоритет: СРЕДНИЙ)

### 3.1 Category Management

**Файлы:**
```
Services/ICategoryService.cs
Services/CategoryService.cs
Controllers/CategoriesController.cs
```

**Endpoints:**
- GET /api/categories
- GET /api/categories/{id}
- POST /api/categories (Admin)
- PUT /api/categories/{id} (Admin)
- DELETE /api/categories/{id} (Admin)

**Время выполнения:** 2-3 часа

---

### 3.2 Manufacturer Management

**Аналогично Category**

**Время выполнения:** 2-3 часа

---

### 3.3 Location Management

**Аналогично Category**

**Время выполнения:** 2-3 часа

---

### 3.4 Supplier Management

**Аналогично Category**

**Время выполнения:** 2-3 часа

---

### 3.5 Status Label Management

**Файлы:**
```
Services/IStatusLabelService.cs
Services/StatusLabelService.cs
Controllers/StatusLabelsController.cs
```

**Время выполнения:** 2-3 часа

---

## 📦 ФАЗА 4: РАСШИРЕННАЯ ФУНКЦИОНАЛЬНОСТЬ (Приоритет: СРЕДНИЙ)

### 4.1 File Upload Service (Загрузка изображений)

**Файлы для создания:**
```
WebShopMercantec/Services/
├── IFileUploadService.cs
└── FileUploadService.cs
```

**Функциональность:**
- Загрузка фото продуктов
- Загрузка аватаров пользователей
- Валидация (размер, формат)
- Сохранение в wwwroot/uploads/
- Генерация уникальных имен файлов

**IFileUploadService:**
```csharp
public interface IFileUploadService
{
    Task<string> UploadImageAsync(IFormFile file, string folder);
    Task<bool> DeleteImageAsync(string imagePath);
    Task<bool> ValidateImageAsync(IFormFile file);
    string GetImageUrl(string imagePath);
}
```

**Добавить endpoints в контроллеры:**
```csharp
[HttpPost("{id}/upload-image")]
[Authorize(Roles = "Admin")]
public async Task<ActionResult> UploadImage(int id, IFormFile file)
```

**Время выполнения:** 3-4 часа

---

### 4.2 Email Service (Уведомления)

**Файлы:**
```
Services/IEmailService.cs
Services/EmailService.cs
Models/EmailSettings.cs (для appsettings)
```

**Установить пакет:**
```bash
dotnet add package MailKit
```

**Функциональность:**
- Уведомление о регистрации
- Уведомление о создании заказа
- Уведомление об одобрении/отклонении заказа
- Уведомление о добавлении кредитов
- Напоминания

**IEmailService:**
```csharp
public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string htmlBody);
    Task SendOrderCreatedAsync(int orderId);
    Task SendOrderApprovedAsync(int orderId);
    Task SendOrderDeclinedAsync(int orderId, string reason);
    Task SendCreditsAddedAsync(int userId, decimal amount);
    Task SendWelcomeEmailAsync(int userId);
}
```

**Время выполнения:** 4-5 часов

---

### 4.3 Search & Filtering Service

**Улучшенный поиск с фильтрацией**

**Файлы:**
```
Services/ISearchService.cs
Services/SearchService.cs
DTOs/SearchFilterDto.cs
DTOs/SearchResultDto.cs
```

**Функциональность:**
- Полнотекстовый поиск
- Фильтр по категориям
- Фильтр по производителям
- Фильтр по цене (min/max credits)
- Сортировка (по цене, по имени, по дате)
- Пагинация

**Endpoints:**
```
GET /api/search?q=laptop&category=3&minPrice=100&maxPrice=500&sort=price_asc&page=1&pageSize=20
```

**Время выполнения:** 4-5 часов

---

### 4.4 Dashboard & Statistics Service

**Для админ-панели**

**Файлы:**
```
Services/IStatisticsService.cs
Services/StatisticsService.cs
Controllers/DashboardController.cs
DTOs/DashboardDto.cs
```

**Статистика:**
- Общее количество пользователей
- Общее количество заказов
- Заказы в ожидании
- Общая сумма кредитов в системе
- Топ-5 популярных товаров
- Топ-5 активных пользователей
- График заказов за последние 30 дней
- График кредитов за последние 30 дней

**Endpoints:**
```csharp
[HttpGet("api/dashboard/statistics")]
[HttpGet("api/dashboard/recent-orders")]
[HttpGet("api/dashboard/top-products")]
[HttpGet("api/dashboard/top-users")]
[HttpGet("api/dashboard/charts/orders")]
[HttpGet("api/dashboard/charts/credits")]
```

**Время выполнения:** 5-6 часов

---

## 📦 ФАЗА 5: ОПТИМИЗАЦИЯ И КАЧЕСТВО КОДА (Приоритет: СРЕДНИЙ)

### 5.1 Validation (Валидация данных)

**Установить:**
```bash
dotnet add package FluentValidation
dotnet add package FluentValidation.AspNetCore
```

**Создать валидаторы:**
```
WebShopMercantec/Validators/
├── LoginDtoValidator.cs
├── RegisterDtoValidator.cs
├── ProductDtoValidator.cs
├── OrderDtoValidator.cs
└── UserDtoValidator.cs
```

**Пример:**
```csharp
public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters")
            .MaximumLength(50);
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"[A-Z]").WithMessage("Password must contain uppercase")
            .Matches(@"[a-z]").WithMessage("Password must contain lowercase")
            .Matches(@"[0-9]").WithMessage("Password must contain digit");
    }
}
```

**Зарегистрировать в Program.cs:**
```csharp
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
```

**Время выполнения:** 3-4 часа

---

### 5.2 Caching (Кеширование)

**Для оптимизации производительности**

**Установить:**
```bash
dotnet add package Microsoft.Extensions.Caching.Memory
dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis (опционально)
```

**Создать:**
```
Services/ICacheService.cs
Services/CacheService.cs
```

**Кешировать:**
- Список категорий
- Список производителей
- Популярные товары
- Настройки системы

**Время выполнения:** 2-3 часа

---

### 5.3 Rate Limiting (Ограничение запросов)

**.NET 9 имеет встроенный Rate Limiting**

**Настроить в Program.cs:**
```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
    });
    
    options.AddPolicy("login", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1)
            }));
});

app.UseRateLimiter();
```

**Применить на endpoints:**
```csharp
[HttpPost("login")]
[EnableRateLimiting("login")]
public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
```

**Время выполнения:** 2 часа

---

### 5.4 Error Handling & Custom Exceptions

**Создать:**
```
Exceptions/
├── NotFoundException.cs
├── UnauthorizedException.cs
├── BadRequestException.cs
├── InsufficientCreditsException.cs
├── ProductNotAvailableException.cs
└── OrderProcessingException.cs
```

**Обновить ErrorHandlingMiddleware** для обработки всех исключений

**Время выполнения:** 2-3 часа

---

### 5.5 API Versioning

**Для будущего масштабирования**

**Установить:**
```bash
dotnet add package Asp.Versioning.Mvc
```

**Настроить:**
```csharp
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// Использовать:
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
```

**Время выполнения:** 1-2 часа

---

## 📦 ФАЗА 6: ТЕСТИРОВАНИЕ (Приоритет: ВЫСОКИЙ)

### 6.1 Unit Tests

**Создать проект:**
```bash
dotnet new xunit -n WebShopMercantec.Tests
dotnet add WebShopMercantec.Tests reference WebShopMercantec/WebShopMercantec.csproj
```

**Установить пакеты:**
```bash
dotnet add package Moq
dotnet add package FluentAssertions
dotnet add package Microsoft.EntityFrameworkCore.InMemory
```

**Создать тесты для:**
- AuthService
- ProductService
- OrderService
- CreditService
- UserService

**Структура:**
```
WebShopMercantec.Tests/
├── Services/
│   ├── AuthServiceTests.cs
│   ├── ProductServiceTests.cs
│   ├── OrderServiceTests.cs
│   └── CreditServiceTests.cs
└── Controllers/
    ├── AuthControllerTests.cs
    └── ProductsControllerTests.cs
```

**Цель:** Минимум 70% покрытие кода

**Время выполнения:** 10-12 часов

---

### 6.2 Integration Tests

**Тестирование API endpoints**

**Создать:**
```
WebShopMercantec.IntegrationTests/
└── Controllers/
    ├── AuthControllerIntegrationTests.cs
    ├── ProductsControllerIntegrationTests.cs
    └── OrdersControllerIntegrationTests.cs
```

**Использовать WebApplicationFactory**

**Время выполнения:** 6-8 часов

---

## 📦 ФАЗА 7: ДОКУМЕНТАЦИЯ И ФИНАЛИЗАЦИЯ (Приоритет: СРЕДНИЙ)

### 7.1 Swagger Configuration (улучшение)

**Обновить конфигурацию Swagger:**

```csharp
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "WebShop Mercantec API",
        Description = "API для системы внутреннего магазина с кредитами",
        Contact = new OpenApiContact
        {
            Name = "Команда разработки",
            Email = "dev@mercantec.dk"
        }
    });
    
    // JWT Authentication в Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    
    // XML комментарии
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});
```

**Время выполнения:** 1-2 часа

---

### 7.2 API Documentation (README для API)

**Создать:**
```
API_DOCUMENTATION.md
```

**Содержание:**
- Описание всех endpoints
- Примеры запросов/ответов
- Коды ошибок
- Схемы аутентификации
- Rate limiting
- Примеры использования

**Время выполнения:** 3-4 часа

---

### 7.3 Database Seeding (Начальные данные)

**Создать:**
```
WebShopMercantec/Data/
├── DbInitializer.cs
└── SeedData.cs
```

**Заполнить:**
- Админ пользователь (admin/admin123)
- Тестовый пользователь (user/user123)
- Категории (Laptops, Monitors, Accessories и т.д.)
- Производители (Dell, HP, Lenovo, Apple)
- Статусы (Ready to Deploy, In Repair, Archived)
- Локации
- Тестовые продукты (5-10 штук)
- Тестовые аксессуары

**Вызвать в Program.cs:**
```csharp
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<SnipeItContext>();
    DbInitializer.Initialize(context);
}
```

**Время выполнения:** 2-3 часа

---

### 7.4 Environment Configuration

**Создать конфигурации для разных окружений:**

**appsettings.Development.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Development connection string"
  },
  "JwtSettings": {
    "SecretKey": "Development_Secret_Key_32_Characters_Min",
    "ExpiryMinutes": 1440
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```

**appsettings.Production.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Production connection string"
  },
  "JwtSettings": {
    "SecretKey": "Production_Secret_From_Environment_Variable",
    "ExpiryMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  }
}
```

**Время выполнения:** 1 час

---

## 📦 ФАЗА 8: БЕЗОПАСНОСТЬ И BEST PRACTICES (Приоритет: КРИТИЧЕСКИЙ)

### 8.1 Security Headers

**Добавить middleware для security headers:**

```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "no-referrer");
    context.Response.Headers.Add("Content-Security-Policy", 
        "default-src 'self'; img-src 'self' data: https:; script-src 'self' 'unsafe-inline'");
    
    await next();
});
```

**Время выполнения:** 1 час

---

### 8.2 CORS Configuration

**Настроить CORS правильно:**

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("https://localhost:7001") // Ваш Blazor клиент
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

app.UseCors("AllowBlazorClient");
```

**Время выполнения:** 1 час

---

### 8.3 Input Sanitization

**Защита от SQL Injection, XSS**

**Создать:**
```
Utils/InputSanitizer.cs
```

**Использовать во всех контроллерах**

**Время выполнения:** 2 часа

---

### 8.4 Password Policy

**Усилить политику паролей:**

```csharp
// В AuthService
private bool ValidatePasswordStrength(string password)
{
    // Минимум 8 символов
    // 1 заглавная буква
    // 1 строчная буква
    // 1 цифра
    // 1 спецсимвол
    var hasMinimum8Chars = password.Length >= 8;
    var hasUpperCase = password.Any(char.IsUpper);
    var hasLowerCase = password.Any(char.IsLower);
    var hasDigit = password.Any(char.IsDigit);
    var hasSpecialChar = password.Any(ch => !char.IsLetterOrDigit(ch));
    
    return hasMinimum8Chars && hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;
}
```

**Время выполнения:** 1 час

---

## 📦 ФАЗА 9: МОНИТОРИНГ И ПРОИЗВОДИТЕЛЬНОСТЬ (Приоритет: НИЗКИЙ)

### 9.1 Health Checks

**Добавить health checks:**

```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<SnipeItContext>()
    .AddCheck("API", () => HealthCheckResult.Healthy());

app.MapHealthChecks("/health");
```

**Время выполнения:** 1 час

---

### 9.2 Performance Monitoring

**Добавить Application Insights (опционально):**

```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

**Время выполнения:** 2-3 часа

---

## 📊 ОБЩАЯ ОЦЕНКА ВРЕМЕНИ

### По приоритетам:

**КРИТИЧЕСКИЙ (обязательно):**
- Фаза 1: Инфраструктура - **16-21 час**
- Фаза 2: Основные сервисы - **28-36 часов**
- Фаза 8: Безопасность - **5-6 часов**

**ВЫСОКИЙ (очень желательно):**
- Фаза 6: Тестирование - **16-20 часов**

**СРЕДНИЙ (желательно):**
- Фаза 3: Доп. сервисы - **10-15 часов**
- Фаза 4: Расширенная функциональность - **16-20 часов**
- Фаза 5: Оптимизация - **10-14 часов**
- Фаза 7: Документация - **7-10 часов**

**НИЗКИЙ (опционально):**
- Фаза 9: Мониторинг - **3-4 часа**

---

## 🎯 ОБЩЕЕ ВРЕМЯ РАЗРАБОТКИ

- **Минимум (только критическое):** 49-63 часа (~1.5-2 недели)
- **Оптимальное (критическое + высокое + среднее):** 110-146 часов (~3-4 недели)
- **Полное (все фазы):** 113-150 часов (~4-5 недель)

---

## 📋 ЧЕКЛИСТ ГОТОВНОСТИ BACKEND

### Authentication & Authorization
- [ ] JWT Token генерация
- [ ] Login endpoint
- [ ] Register endpoint
- [ ] Refresh token mechanism
- [ ] Password hashing (BCrypt)
- [ ] Role-based authorization
- [ ] Current user endpoint

### User Management
- [ ] CRUD операции для пользователей
- [ ] Получение профиля пользователя
- [ ] Обновление профиля
- [ ] Управление кредитами
- [ ] История транзакций

### Credit System
- [ ] Добавление кредитов
- [ ] Списание кредитов
- [ ] История транзакций
- [ ] Проверка баланса
- [ ] Refund mechanism

### Product Management
- [ ] Список доступных товаров
- [ ] Детали товара
- [ ] Поиск и фильтрация
- [ ] Пагинация
- [ ] CRUD (Admin)
- [ ] Accessories support

### Order Management
- [ ] Создание заказа
- [ ] Отмена заказа
- [ ] Одобрение заказа (Admin)
- [ ] Отклонение заказа (Admin)
- [ ] История заказов
- [ ] Статистика заказов

### Infrastructure
- [ ] Repository Pattern
- [ ] Unit of Work
- [ ] Error Handling Middleware
- [ ] Logging (Serilog)
- [ ] Validation (FluentValidation)
- [ ] Caching
- [ ] Rate Limiting

### Security
- [ ] Security Headers
- [ ] CORS configuration
- [ ] Input sanitization
- [ ] SQL Injection protection
- [ ] XSS protection
- [ ] Password policy

### Documentation
- [ ] Swagger configuration
- [ ] XML comments на всех endpoints
- [ ] API Documentation (README)
- [ ] Примеры использования

### Testing
- [ ] Unit tests (70%+ coverage)
- [ ] Integration tests
- [ ] Тесты для всех критических сервисов

### Additional Features
- [ ] File upload (images)
- [ ] Email notifications
- [ ] Dashboard statistics
- [ ] Search functionality
- [ ] Health checks

---

## 🚀 РЕКОМЕНДУЕМЫЙ ПОРЯДОК ВЫПОЛНЕНИЯ

### Неделя 1: Foundation
1. Repository Pattern + Unit of Work (День 1-2)
2. Authentication + Authorization (День 3-4)
3. Error Handling + Logging (День 5)

### Неделя 2: Core Features
1. User Service + Controller (День 1)
2. Credit System (День 2-3)
3. Product Service расширение (День 4)
4. Order Service + Controller (День 5)

### Неделя 3: Additional Services
1. Accessory Service (День 1)
2. Category, Manufacturer, Location Services (День 2)
3. File Upload (День 3)
4. Email Service (День 4)
5. Search & Filtering (День 5)

### Неделя 4: Quality & Testing
1. Validation (День 1)
2. Security improvements (День 2)
3. Unit Tests (День 3-4)
4. Integration Tests + Documentation (День 5)

---

## 💡 СОВЕТЫ И ЛУЧШИЕ ПРАКТИКИ

### 1. Начните с малого
Не пытайтесь сделать всё сразу. Фокусируйтесь на одной фазе за раз.

### 2. Тестируйте постоянно
Пишите unit tests сразу после создания сервиса, не откладывайте на потом.

### 3. Используйте Swagger
Проверяйте каждый endpoint в Swagger после создания.

### 4. Git commits
Делайте частые коммиты с понятными сообщениями:
- `feat: add authentication service`
- `fix: resolve credit deduction bug`
- `refactor: improve order service logic`

### 5. Code Review
Если есть возможность, просите коллег проверить критичные части кода.

### 6. Performance
Используйте AsNoTracking() для read-only операций.
Добавляйте индексы в БД на часто используемые поля.

### 7. Security First
Никогда не храните пароли в открытом виде.
Всегда валидируйте входные данные.
Используйте параметризованные запросы (EF Core делает это автоматически).

### 8. Документация
Пишите XML комментарии сразу. Потом будет лень.

### 9. Async/Await
Используйте async/await для всех I/O операций (БД, файлы, email).

### 10. DRY (Don't Repeat Yourself)
Если копируете код более 2 раз - создайте helper метод.

---

## 🎓 ЗАКЛЮЧЕНИЕ

Этот роадмап покрывает **100% backend разработки** для вашего проекта WebShopMercantec. 

**Критические фазы (1, 2, 8)** дадут вам работающий продукт.
**Остальные фазы** сделают продукт production-ready, масштабируемым и maintainable.

Удачи в разработке! 🚀

---

**Создано:** 14 декабря 2025
**Версия:** 1.0
**Автор:** GitHub Copilot

