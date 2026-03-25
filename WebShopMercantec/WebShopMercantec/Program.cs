using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Components;
using WebShopMercantec.Models;
using WebShopMercantec.Services;
using WebShopMercantec.Repositories;
using WebShopMercantec.Repositories.Specific;
using WebShopMercantec.Middleware;
using WebShopMercantec.Configuration;
using Serilog;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace WebShopMercantec;
public class Program
{
    public static void Main(string[] args)
    {
        // === НАСТРОЙКА SERILOG ===
        // Конфигурируем Serilog ДО создания builder (используем централизованный класс)
        SerilogConfiguration.ConfigureSerilog();

        try
        {
            Log.Information("Starting WebShopMercantec application");
            
            var builder = WebApplication.CreateBuilder(args);

            // Используем Serilog вместо стандартного логирования
            builder.Host.UseSerilog();

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();
        
        // Добавляем поддержку контроллеров (для API)
        builder.Services.AddControllers();
        
        // === CORS ===
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("WebShopPolicy", policy =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                }
                else
                {
                    policy.WithOrigins(
                              builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() 
                              ?? new[] { "https://localhost" })
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                }
            });
        });
        
        //db
        // Получаем строку подключения — падаем сразу с понятным сообщением если пусто
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "ConnectionStrings:DefaultConnection is empty. " +
                "Set it in appsettings.Development.json or via environment variable " +
                "ConnectionStrings__DefaultConnection.");
        }

        // Регистрируем контекст с retry для стабильности через SSH туннель
        builder.Services.AddDbContext<SnipeItContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                mysqlOptions => mysqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null)));
        
        // === REPOSITORY PATTERN ===
        // Generic Repository
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        
        // Регистрируем специфичные репозитории
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<IAccessoryRepository, AccessoryRepository>();
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        builder.Services.AddScoped<IManufacturerRepository, ManufacturerRepository>();
        builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
        builder.Services.AddScoped<ILocationRepository, LocationRepository>();
        builder.Services.AddScoped<IStatusLabelRepository, StatusLabelRepository>();
        
        // Регистрируем Unit of Work (главный координатор всех репозиториев)
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        // === END REPOSITORY PATTERN ===
        
        // === FLUENT VALIDATION ===
        // Автоматическая регистрация всех валидаторов из сборки
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();
        builder.Services.AddFluentValidationAutoValidation();
        // === END VALIDATION ===

        // === RATE LIMITING ===
        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddFixedWindowLimiter("auth", limiterOptions =>
            {
                limiterOptions.PermitLimit = 5;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueLimit = 0;
            });
        });
        // === END RATE LIMITING ===
        
        // === SERVICES ===
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<IManufacturerService, ManufacturerService>();
        builder.Services.AddScoped<ISupplierService, SupplierService>();
        builder.Services.AddScoped<ILocationService, LocationService>();
        builder.Services.AddScoped<IStatusLabelService, StatusLabelService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ICreditService, CreditService>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        // === END SERVICES ===

        // === JWT AUTHENTICATION ===
        var jwtSettings = builder.Configuration
            .GetSection(JwtSettings.SectionName)
            .Get<JwtSettings>() ?? new JwtSettings();

        if (string.IsNullOrWhiteSpace(jwtSettings.Key))
        {
            throw new InvalidOperationException(
                "Jwt:Key is empty. Set it in appsettings.Development.json or via " +
                "environment variable Jwt__Key. Minimum 32 characters required.");
        }

        if (jwtSettings.Key.Length < 32)
        {
            throw new InvalidOperationException(
                $"Jwt:Key is too short ({jwtSettings.Key.Length} chars). Minimum 32 characters required.");
        }

        // Register as singleton so TokenService can inject it directly
        builder.Services.AddSingleton(jwtSettings);

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Key)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        builder.Services.AddAuthorization();
        // === END JWT ===
        
        // === HEALTH CHECKS ===
        builder.Services.AddHealthChecks()
            .AddDbContextCheck<SnipeItContext>("database");
        // === END HEALTH CHECKS ===
        
       //Swagger с поддержкой JWT авторизации
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            var xmlFile = "WebShopMercantec.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath);
        });

        var app = builder.Build();

        // === ERROR HANDLING MIDDLEWARE ===
        // ВАЖНО: Должен быть ПЕРВЫМ в pipeline!
        // Перехватывает все исключения из последующих middleware
        app.UseErrorHandling();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
            app.UseSwagger(); // Генерирует JSON файл
            app.UseSwaggerUI();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseCors("WebShopPolicy");

        app.UseRateLimiter();

        // JWT Authentication + Role-based Authorization
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseAntiforgery();

        app.MapStaticAssets();
        
        // Регистрируем маршруты для API контроллеров
        app.MapControllers();
        
        // Health check endpoint (используется docker healthcheck)
        app.MapHealthChecks("/health");
        
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

        app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}