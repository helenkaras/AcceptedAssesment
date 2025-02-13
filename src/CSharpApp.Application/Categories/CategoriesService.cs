using CSharpApp.Core.Models;
using System.Net.Http.Json;

namespace CSharpApp.Application.Categories
{
    public class CategoriesService : ICategoryService
    {
        private readonly HttpClient _httpClient;
        private readonly RestApiSettings _restApiSettings;
        private readonly ILogger<CategoriesService> _logger;

        public CategoriesService(HttpClient httpClient, IOptions<RestApiSettings> restApiSettings,
            ILogger<CategoriesService> logger)
        {
            _httpClient = httpClient;
            _restApiSettings = restApiSettings.Value;
            _logger = logger;
        }

        public async Task<IReadOnlyCollection<CategoryApiResponse>> GetCategories()
        {
            try
            {
                _logger.LogInformation("Retrieving categories");

                var response = await _httpClient.GetAsync(_restApiSettings.Categories);
                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error: {errorMessage.Result}");
                }
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var categories = JsonSerializer.Deserialize<List<CategoryApiResponse>>(content);

                return categories.AsReadOnly();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed while retrieving categories");
                throw;
            }
        }
        public async Task<CategoryApiResponse> GetCategoryById(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving Category with ID: {CategoryId}", id);

                var response = await _httpClient.GetAsync($"{_restApiSettings.Categories}/{id}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var category = JsonSerializer.Deserialize<CategoryApiResponse>(content);

                return category;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed while retrieving product");
                throw;
            }
        }
        public async Task<CategoryApiResponse> CreateCategory(Category category)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_restApiSettings.Categories}/", category);
                _logger.LogInformation("Response status: {StatusCode}", response.StatusCode);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error: {errorMessage.Result}");
                }

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<CategoryApiResponse>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed while creating category");
                throw;
            }
        }
    }
}
