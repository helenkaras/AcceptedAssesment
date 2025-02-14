
namespace CSharpApp.Core.Interfaces
{
    public interface IAuthService
    {
        Task<string> GetAccessToken();
    }
}
