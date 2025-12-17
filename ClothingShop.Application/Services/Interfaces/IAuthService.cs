using ClothingShop.Application.DTOs.Auth;
using ClothingShop.Application.Wrapper;

namespace ClothingShop.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request);
        Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
        Task<ApiResponse<LoginResponse>> RefreshTokenAsync(string refreshToken);
    }
}
