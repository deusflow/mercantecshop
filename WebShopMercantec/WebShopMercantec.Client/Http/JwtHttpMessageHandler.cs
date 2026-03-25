using System.Net;
using System.Net.Http.Headers;
using WebShopMercantec.Client.Auth;

namespace WebShopMercantec.Client.Http;

public sealed class JwtHttpMessageHandler(ITokenStore tokenStore, AuthStateProvider authStateProvider) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await tokenStore.GetAccessTokenAsync();
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await authStateProvider.MarkUserLoggedOutAsync();
        }

        return response;
    }
}

