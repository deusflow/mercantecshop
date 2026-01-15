using Microsoft.JSInterop;

namespace WebShopMercantec.Services
{
    public partial class APIService
    {
        public async Task<bool> Login(LoginDto user)
        {
            try { 
                var response = await _httpClient.PostAsJsonAsync("api/Auth/login", user);

                if(!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Login failed: {response.StatusCode} - {errorContent}");
                    return false;
                }

                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

                if (result == null || string.IsNullOrEmpty(result.AccessToken))
                    return false;

                await _js.InvokeVoidAsync("localStorage.setItem", "access_token", result.AccessToken);

                Console.WriteLine("✅ Login successful with refresh token");
                return true;
            } catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n\nStack trace: {ex.StackTrace}");
                return false;
            }
        }
    }
}
