using IDMS.Web.Middleware;

namespace IDMS.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApiClient _api;

        public AuthService(ApiClient api)
        {
            _api = api;
        }

        public async Task<(bool Success, string? Token, string? Email, string? Error)> LoginAsync(string email, string password)
        {
            try
            {
                var result = await _api.PostAsync<LoginData>("/api/auth/login", new { email, password });

                if (result.Status == "Error")
                    return (false, null, null, result.Message);

                return (true, result.Data?.Token, result.Data?.Email, null);
            }
            catch (ApiException ex)
            {
                return (false, null, null, ex.Message);
            }
            catch (HttpRequestException ex)
            {
                return (false, null, null, $"Connection failed: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? Error)> RegisterAsync(string email, string password, string fullName)
        {
            try
            {
                var result = await _api.PostAsync<object>("/api/auth/register", new { email, password, fullName });

                if (result.Status == "Error")
                    return (false, result.Message);

                return (true, null);
            }
            catch (ApiException ex)
            {
                return (false, ex.Message);
            }
            catch (HttpRequestException ex)
            {
                return (false, $"Connection failed: {ex.Message}");
            }
        }
    }

    public class LoginData
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
