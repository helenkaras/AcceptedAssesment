using System.Net;
using System.Text.Json;
using Moq;
using Moq.Protected;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CSharpApp.Application.Categories;
using CSharpApp.Core.Models;
using CSharpApp.Core.Settings;
using CSharpApp.Core.Dtos;

namespace CSharpApp.Application.Tests { 
public class CategoriesServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly Mock<IOptions<RestApiSettings>> _settingsMock;
    private readonly Mock<ILogger<CategoriesService>> _loggerMock;
    private readonly CategoriesService _categoriesService;

    public CategoriesServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://api.example.com/")
        };

        _settingsMock = new Mock<IOptions<RestApiSettings>>();
        _settingsMock.Setup(s => s.Value).Returns(new RestApiSettings { Categories = "categories" });

        _loggerMock = new Mock<ILogger<CategoriesService>>();

        _categoriesService = new CategoriesService(_httpClient, _settingsMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetCategories_ReturnsListOfCategories()
    {
        // Arrange
        var fakeCategories = new List<CategoryApiResponse>
    {
        new CategoryApiResponse { Id = 1, Name = "Electronics" },
        new CategoryApiResponse { Id = 2, Name = "Clothing" }
    };

        var responseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(fakeCategories))
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _categoriesService.GetCategories();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Electronics", result.First().Name);
    }

        [Fact]
        public async Task GetCategoryById_ReturnsCategory()
        {
            // Arrange
            var categoryId = 1;
            var fakeCategory = new CategoryApiResponse { Id = categoryId, Name = "Books", Image = "https://example.com/book.jpg" };

            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(fakeCategory))
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(responseMessage);

            // Act
            var result = await _categoriesService.GetCategoryById(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categoryId, result.Id);
            Assert.Equal("Books", result.Name);
            Assert.Equal("https://example.com/book.jpg", result.Image);
        }

        [Fact]
    public async Task CreateCategory_ReturnsCreatedCategory()
    {
        // Arrange
        var newCategory = new Category
        {
            Name = "Sports",
            Image = "https://example.com/sports.jpg"
        };

        var fakeResponse = new CategoryApiResponse
        {
            Id = 5,
            Name = "Sports",
            Image = "https://example.com/sports.jpg"
        };

        var responseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.Created,
            Content = new StringContent(JsonSerializer.Serialize(fakeResponse))
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _categoriesService.CreateCategory(newCategory);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Id);
        Assert.Equal("Sports", result.Name);
    }
}
}
