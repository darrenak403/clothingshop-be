using ClothingShop.Application.DTOs.Auth;
using ClothingShop.Application.Wrapper;

namespace ClothingShop.Application.Services.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request);
        Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
        Task<ApiResponse<LoginResponse>> RefreshTokenAsync(string refreshToken);
        Task<ApiResponse<string>> LogoutAsync(string refreshToken);
        Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordRequest request);
        Task<ApiResponse<string>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    }
}
