using CSharpApp.Core.Models;
using System.Net.Http.Json;

namespace CSharpApp.Application.Auth
{
    public class AuthService: IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly RestApiSettings _restApiSettings;
        private readonly ILogger<AuthService> _logger;
        private string? _cachedToken;
        private DateTime _tokenExpiry = DateTime.MinValue;

        public AuthService(HttpClient httpClient, IOptions<RestApiSettings> restApiSettings, ILogger<AuthService> logger)
        {
            _httpClient = httpClient;
            _restApiSettings = restApiSettings.Value;
            _logger = logger;
        }

        private async Task<AuthApiResponse> Login(string email, string password)
        {
            try
            {
                var loginData = new { email, password };
                var response = await _httpClient.PostAsJsonAsync($"{_restApiSettings.Auth}/", loginData);
                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error: {errorMessage.Result}");
                }
                response.EnsureSuccessStatusCode();

                var authResponse = await response.Content.ReadFromJsonAsync<AuthApiResponse>();
                if (authResponse.AccessToken == null || authResponse.RefreshToken == null)
                {
                    throw new Exception("Failed to deserialize auth response");
                }

                _cachedToken = authResponse.AccessToken;
                _tokenExpiry = DateTime.UtcNow.AddDays(19); // Token valid for 20 days, we refresh 1 day earlier

                return authResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to authenticate");
                throw;
            }
        }

        public async Task<string> GetAccessToken()
        {
            if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiry)
            {
                return _cachedToken;
            }

            var authResponse = await Login(_restApiSettings.Username, _restApiSettings.Password);
            return authResponse.AccessToken;
        }
    }
}
