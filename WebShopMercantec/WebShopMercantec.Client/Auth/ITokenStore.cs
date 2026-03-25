namespace WebShopMercantec.Client.Auth;

public interface ITokenStore
{
    ValueTask<string?> GetAccessTokenAsync();
    ValueTask<string?> GetRefreshTokenAsync();
    ValueTask SetTokensAsync(string accessToken, string refreshToken);
    ValueTask ClearAsync();
}

