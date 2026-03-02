using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Client.Pages;
using WebShopMercantec.Components;
using WebShopMercantec.Models;
using WebShopMercantec.Services;
using WebShopMercantec.Repositories;
using WebShopMercantec.Repositories.Specific;
using WebShopMercantec.Middleware;
using WebShopMercantec.Configuration;
using Serilog;
using FluentValidation;


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
        
        // подтягиваю строку подключения 
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

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
        // === END VALIDATION ===
        
        // === SERVICES ===
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<IManufacturerService, ManufacturerService>();
        builder.Services.AddScoped<ISupplierService, SupplierService>();
        builder.Services.AddScoped<ILocationService, LocationService>();
        builder.Services.AddScoped<IStatusLabelService, StatusLabelService>();
        // === END SERVICES ===
        
        // === HEALTH CHECKS ===
        builder.Services.AddHealthChecks()
            .AddDbContextCheck<SnipeItContext>("database");
        // === END HEALTH CHECKS ===
        
       //Swaaaaagger maaa boy
        builder.Services.AddEndpointsApiExplorer(); // Нужно для Minimal API
        builder.Services.AddSwaggerGen(options =>
        {
            var xmlFile = "WebShopMercantec.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
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