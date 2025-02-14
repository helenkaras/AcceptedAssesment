using CSharpApp.Api.Middleware;

namespace CSharpApp.Api.MiddlewareExtension
{
    public static class RequestPerformanceMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestPerformance(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestPerformanceLogger>();
        }
    }
}
