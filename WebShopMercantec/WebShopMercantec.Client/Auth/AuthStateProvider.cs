using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace WebShopMercantec.Client.Auth;

public sealed class AuthStateProvider(ITokenStore tokenStore) : AuthenticationStateProvider
{
    private static readonly AuthenticationState Anonymous =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await tokenStore.GetAccessTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
        {
            return Anonymous;
        }

        var identity = BuildIdentityFromJwt(token);
        if (identity is null)
        {
            return Anonymous;
        }

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task MarkUserAuthenticatedAsync(string accessToken, string refreshToken)
    {
        await tokenStore.SetTokensAsync(accessToken, refreshToken);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task MarkUserLoggedOutAsync()
    {
        await tokenStore.ClearAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }

    private static ClaimsIdentity? BuildIdentityFromJwt(string token)
    {
        try
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var claims = jwt.Claims.ToList();

            foreach (var roleClaim in jwt.Claims.Where(c => c.Type is "role" or "roles"))
            {
                claims.Add(new Claim(ClaimTypes.Role, roleClaim.Value));
            }

            return new ClaimsIdentity(claims, authenticationType: "jwt");
        }
        catch
        {
            return null;
        }
    }
}

