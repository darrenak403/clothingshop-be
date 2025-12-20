using ClothingShop.Application.DTOs.User;
using ClothingShop.Application.Wrapper;

namespace ClothingShop.Application.Services.UserProfile.Interfaces
{
    public interface IUserService
    {
        // --- Dành cho Member (Cá nhân User tự thao tác) ---
        Task<ApiResponse<UserProfileDto>> GetMyProfileAsync(Guid userId);
        Task<ApiResponse<bool>> UpdateMyProfileAsync(Guid userId, UpdateProfileRequest request);

        // --- Dành cho Admin (Quản trị hệ thống) ---
        Task<ApiResponse<UserDto>> GetUserByIdAsync(Guid userId);
        Task<ApiResponse<PagedResult<UserDto>>> GetAllUsersAsync(UserFilterRequest query);
        // Khóa/Mở khóa -> Trả về bool kèm message
        Task<ApiResponse<bool>> ToggleUserStatusAsync(Guid userId, bool isActive, string? reason);
    }
}