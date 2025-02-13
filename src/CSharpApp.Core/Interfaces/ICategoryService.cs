using CSharpApp.Core.Models;

public interface ICategoryService
{
    Task<IReadOnlyCollection<CategoryApiResponse>> GetCategories();
    Task<CategoryApiResponse> GetCategoryById(int id);
    Task<CategoryApiResponse> CreateCategory(Category category);


}
