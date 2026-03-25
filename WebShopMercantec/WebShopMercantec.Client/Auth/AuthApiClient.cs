using System.Net.Http.Json;
using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Client.Auth;

public sealed class AuthApiClient(HttpClient http)
{
    public async Task<AuthResponseDto> LoginAsync(LoginDto model, CancellationToken cancellationToken = default)
    {
        var response = await http.PostAsJsonAsync("/api/auth/login", model, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Login failed ({(int)response.StatusCode})", null, response.StatusCode);
        }

        var payload = await response.Content.ReadFromJsonAsync<AuthResponseDto>(cancellationToken);
        if (payload is null || string.IsNullOrWhiteSpace(payload.AccessToken) || string.IsNullOrWhiteSpace(payload.RefreshToken))
        {
            throw new InvalidOperationException("Invalid login response payload.");
        }

        return payload;
    }
}

