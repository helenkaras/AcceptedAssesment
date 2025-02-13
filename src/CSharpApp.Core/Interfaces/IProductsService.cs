using CSharpApp.Core.Models;

namespace CSharpApp.Core.Interfaces;

public interface IProductsService
{
    Task<IReadOnlyCollection<Product>> GetProducts();
    Task<Product> GetProductById(int id);
    Task<ProductApiResponse> CreateProduct(Product product);
}