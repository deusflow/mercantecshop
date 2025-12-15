using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Client.Pages;
using WebShopMercantec.Components;
using WebShopMercantec.Models;
using System.IO;
using WebShopMercantec.Services;
using WebShopMercantec.Repositories;
using WebShopMercantec.Repositories.Specific;
using WebShopMercantec.Middleware;
using Serilog;
using FluentValidation;



namespace WebShopMercantec;
public class Program
{
    public static void Main(string[] args)
    {
        // === НАСТРОЙКА SERILOG ===
        // Конфигурируем Serilog ДО создания builder
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(
                "logs/webshop-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30)
            .CreateLogger();

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
        
        //db
        
        // подтягиваю строку подключения 
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        // Регистрируем контекст 
        builder.Services.AddDbContext<SnipeItContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
        
        // === REPOSITORY PATTERN ===
        // Generic Repository
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        
        // Регистрируем специфичные репозитории
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<IAccessoryRepository, AccessoryRepository>();
        
        // Регистрируем Unit of Work (главный координатор всех репозиториев)
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        // === END REPOSITORY PATTERN ===
        
        // === FLUENT VALIDATION ===
        // Автоматическая регистрация всех валидаторов из сборки
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();
        // === END VALIDATION ===
        
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
        builder.Services.AddScoped<IProductService, ProductService>();

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

        app.UseAntiforgery();

        app.MapStaticAssets();
        
        // Регистрируем маршруты для API контроллеров
        app.MapControllers();
        
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);
        
        // Временный тест: Получить первые 5 ассетов из базы
        app.MapGet("/test-assets", async (SnipeItContext db) =>
            {
                // Берем 5 штук, чтобы не грузить всю базу
                return await db.Assets.Take(5).ToListAsync();
            })
            .WithName("GetAssets");

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