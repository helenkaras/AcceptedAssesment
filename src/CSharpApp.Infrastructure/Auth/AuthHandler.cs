using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace CSharpApp.Infrastructure.Auth
{
    public class AuthHandler : DelegatingHandler
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthHandler> _logger;

        public AuthHandler(IAuthService authService, ILogger<AuthHandler> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var token = await _authService.GetAccessToken();
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                return await base.SendAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in JWT authentication handler");
                throw;
            }
        }
    }
}
