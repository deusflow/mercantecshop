using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Web;
using WebShopMercantec.Client.Auth;
using WebShopMercantec.Client.Http;

using WebShopMercantec.Client.Components;

namespace WebShopMercantec.Client;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<Routes>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddAuthorizationCore();

        builder.Services.AddScoped<ITokenStore, BrowserTokenStore>();
        builder.Services.AddScoped<AuthStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<AuthStateProvider>());
        builder.Services.AddScoped<JwtHttpMessageHandler>();

        builder.Services.AddScoped(sp =>
        {
            var handler = sp.GetRequiredService<JwtHttpMessageHandler>();
            handler.InnerHandler = new HttpClientHandler();
            return new HttpClient(handler)
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
            };
        });

        builder.Services.AddScoped<AuthApiClient>();

        await builder.Build().RunAsync();
    }
}