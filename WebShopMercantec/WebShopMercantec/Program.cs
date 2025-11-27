using Microsoft.EntityFrameworkCore; // <-- Добавьте эту строку
using WebShopMercantec.Client.Pages;
using WebShopMercantec.Components;
using WebShopMercantec.Models;
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
        //db
        
       var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
       builder.Services.AddDbContext<SnipeItContext>(options =>
           options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
       );
       

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
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
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

        app.Run();
    }
}