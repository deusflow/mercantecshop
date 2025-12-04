using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Client.Pages;
using WebShopMercantec.Components;
using WebShopMercantec.Models;
using System.IO;



namespace WebShopMercantec;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

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
        
       //Swaaaaagger maaa boy
        builder.Services.AddEndpointsApiExplorer(); // Нужно для Minimal API
        builder.Services.AddSwaggerGen(options =>
        {
            // ...existing configuration...
            var xmlFile = "WebShopMercantec.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        });

        var app = builder.Build();

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
}