using CSharpApp.Core.Models;
using System.Net.Http.Json;

namespace CSharpApp.Application.Products;

public class ProductsService : IProductsService
{
    private readonly HttpClient _httpClient;
    private readonly RestApiSettings _restApiSettings;
    private readonly ILogger<ProductsService> _logger;

    public ProductsService(HttpClient httpClient, IOptions<RestApiSettings> restApiSettings, 
        ILogger<ProductsService> logger)
    {
        _httpClient = httpClient;
        _restApiSettings = restApiSettings.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Product>> GetProducts()
    {
        try
        {
            _logger.LogInformation("Retrieving products");

            var response = await _httpClient.GetAsync(_restApiSettings.Products);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<List<Product>>(content);

            return products.AsReadOnly();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed while retrieving products");
            throw;
        }
    }
    public async Task<Product> GetProductById(int id)
    {
        try
        {
            _logger.LogInformation("Retrieving product with ID: {ProductId}", id);

            var response = await _httpClient.GetAsync($"{_restApiSettings.Products}/{id}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<Product>(content);

            return product;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed while retrieving product");
            throw;
        }
    }
    public async Task<ProductApiResponse> CreateProduct(Product product)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_restApiSettings.Products}/", product);
            _logger.LogInformation("Response status: {StatusCode}", response.StatusCode);

            if (!response.IsSuccessStatusCode) {
                var errorMessage = response.Content.ReadAsStringAsync();
               _logger.LogError($"Error: {errorMessage.Result}");
            }

             response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ProductApiResponse>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed while creating product");
            throw;
        }
    }
}