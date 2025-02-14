using CSharpApp.Application.Auth;
using CSharpApp.Application.Categories;
using CSharpApp.Application.Products;
using CSharpApp.Core.Dtos;
using CSharpApp.Core.Settings;
using CSharpApp.Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Logging.ClearProviders().AddSerilog(logger);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDefaultConfiguration();
builder.Services.AddHttpConfiguration();
builder.Services.AddProblemDetails();
builder.Services.AddApiVersioning();


builder.Services.AddHttpClient<IProductsService, ProductsService>((sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<RestApiSettings>>().Value;
    client.BaseAddress = new Uri(builder.Configuration["RestApiSettings:BaseUrl"]);
}).AddHttpMessageHandler<AuthHandler>();

builder.Services.AddHttpClient<ICategoryService, CategoriesService>((sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<RestApiSettings>>().Value;
    client.BaseAddress = new Uri(builder.Configuration["RestApiSettings:BaseUrl"]);
}).AddHttpMessageHandler<AuthHandler>();

builder.Services.AddHttpClient<IAuthService, AuthService>((sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<RestApiSettings>>().Value;
    client.BaseAddress = new Uri(builder.Configuration["RestApiSettings:BaseUrl"]);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

var versionedEndpointRouteBuilder = app.NewVersionedApi();

versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/getproducts", async (IProductsService productsService) =>
    {
        var products = await productsService.GetProducts();
        return products;
    })
    .WithName("GetProducts")
.HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/getproduct/{id}", async (int id, IProductsService productsService) =>
{
    var product = await productsService.GetProductById(id);
    return product is not null ? Results.Ok(product) : Results.NotFound();
})
    .WithName("GetProductById")
    .HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapPost("api/v{version:apiVersion}/createproducts", async ([FromBody] Product product, IProductsService productsService) =>
{
    var createdProduct = await productsService.CreateProduct(product);
    return Results.Created($"api/v1/products/{createdProduct.Id}", createdProduct);
})
.WithName("CreateProduct")
.HasApiVersion(1.0);


versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/getcategories", async (ICategoryService categoryService) =>
{
    var categories = await categoryService.GetCategories();
    return categories is not null ? Results.Ok(categories) : Results.NotFound(); ;
})
    .WithName("GetCategory")
.HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/getcategory/{id}", async (int id, ICategoryService categoryService) =>
{
    var category = await categoryService.GetCategoryById(id);
    return category is not null ? Results.Ok(category) : Results.NotFound();
})
    .WithName("GetCategoryById")
    .HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapPost("api/v{version:apiVersion}/createcategory", async ([FromBody] Category category, ICategoryService categoryService) =>
{
    var createdCategory = await categoryService.CreateCategory(category);
    return Results.Created($"api/v1/products/{createdCategory.Id}", createdCategory);
})
.WithName("CreateCategory")
.HasApiVersion(1.0);

app.Run();