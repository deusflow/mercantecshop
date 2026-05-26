using Microsoft.JSInterop;

namespace WebShopMercantec.Client.Auth;

public sealed class BrowserTokenStore(IJSRuntime js) : ITokenStore
{
    private const string AccessKey = "ws.accessToken";
    private const string RefreshKey = "ws.refreshToken";

    public ValueTask<string?> GetAccessTokenAsync() =>
        js.InvokeAsync<string?>("localStorage.getItem", AccessKey);

    public ValueTask<string?> GetRefreshTokenAsync() =>
        js.InvokeAsync<string?>("localStorage.getItem", RefreshKey);

    public async ValueTask SetTokensAsync(string accessToken, string refreshToken)
    {
        await js.InvokeVoidAsync("localStorage.setItem", AccessKey, accessToken);
        await js.InvokeVoidAsync("localStorage.setItem", RefreshKey, refreshToken);
    }

    public async ValueTask ClearAsync()
    {
        await js.InvokeVoidAsync("localStorage.removeItem", AccessKey);
        await js.InvokeVoidAsync("localStorage.removeItem", RefreshKey);
    }
}

