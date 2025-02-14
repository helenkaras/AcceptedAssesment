
namespace CSharpApp.Api.Middleware
{
    public class RequestPerformanceLogger
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestPerformanceLogger> _logger;

        public RequestPerformanceLogger(RequestDelegate next, ILogger<RequestPerformanceLogger> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var startTime = DateTime.UtcNow;

            try
            {
                await _next(context);
            }
            finally
            {
                var endTime = DateTime.UtcNow;
                var duration = (endTime - startTime).TotalMilliseconds;

                var requestPath = context.Request.Path;
                var method = context.Request.Method;
                var statusCode = context.Response.StatusCode;

                _logger.LogInformation(
                    "Request {Method} {Path} completed in {Duration:0.0}ms with status code {StatusCode}",
                    method,
                    requestPath,
                    duration,
                    statusCode
                );
            }
        }
    }
}
