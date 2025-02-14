using CSharpApp.Application.Products;
using CSharpApp.Core.Dtos;
using CSharpApp.Core.Models;
using CSharpApp.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace CSharpApp.Application.Tests
{
    public class ProductsServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly Mock<IOptions<RestApiSettings>> _settingsMock;
        private readonly Mock<ILogger<ProductsService>> _loggerMock;
        private readonly ProductsService _productsService;

        public ProductsServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://api.example.com/")
            };

            _settingsMock = new Mock<IOptions<RestApiSettings>>();
            _settingsMock.Setup(s => s.Value).Returns(new RestApiSettings { Products = "products" });

            _loggerMock = new Mock<ILogger<ProductsService>>();

            _productsService = new ProductsService(_httpClient, _settingsMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetProducts_ReturnsProductList()
        {
            // Arrange
            var fakeProducts = new List<Product>
        {
            new Product { Id = 1, Title = "Product 1", Price = 10 },
            new Product { Id = 2, Title = "Product 2", Price = 20 }
        };
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(fakeProducts))
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
            var result = await _productsService.GetProducts();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Product 1", result.First().Title);
        }


        [Fact]
        public async Task GetProductById_ReturnsProduct()
        {
            // Arrange
            var productId = 1;
            var fakeProduct = new Product { Id = productId, Title = "Test Product", Price = 99 };
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(fakeProduct))
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
            var result = await _productsService.GetProductById(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.Id);
            Assert.Equal("Test Product", result.Title);
        }

        [Fact]
        public async Task CreateProduct_ReturnsCreatedProduct()
        {
            // Arrange
            var newProduct = new Product
            {
                Title = "New Product",
                Price = 50,
                Description = "A test product",
                Images = new List<string> { "https://example.com/image.jpg" }
            };

            var fakeResponse = new ProductApiResponse
            {
                Id = 10,
                Title = "New Product",
                Price = 50,
                Description = "A test product",
                Images = new List<string> { "https://example.com/image.jpg" }
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
            var result = await _productsService.CreateProduct(newProduct);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.Id);
            Assert.Equal("New Product", result.Title);
            Assert.Equal(50, result.Price);
        }
    }
}
