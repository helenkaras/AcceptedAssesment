using CSharpApp.Application.Auth;
using CSharpApp.Application.Categories;
using CSharpApp.Infrastructure.Auth;

namespace CSharpApp.Infrastructure.Configuration;

public static class DefaultConfiguration
{
    public static IServiceCollection AddDefaultConfiguration(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetService<IConfiguration>();

        services.Configure<RestApiSettings>(configuration!.GetSection(nameof(RestApiSettings)));
        services.Configure<HttpClientSettings>(configuration.GetSection(nameof(HttpClientSettings)));

        services.AddScoped<IProductsService, ProductsService>();
        services.AddScoped<ICategoryService, CategoriesService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddTransient<AuthHandler>();

        return services;
    }
}