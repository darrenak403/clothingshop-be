using ClothingShop.Application.DTOs.Auth;

namespace ClothingShop.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto request); // Trả về Token hoặc Success Message
        Task<string> LoginAsync(LoginDto request);       // Trả về Token
    }
}
