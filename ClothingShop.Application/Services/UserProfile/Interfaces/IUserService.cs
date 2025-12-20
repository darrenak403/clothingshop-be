using ClothingShop.Application.DTOs.User;
using ClothingShop.Application.Wrapper;

namespace ClothingShop.Application.Services.UserProfile.Interfaces
{
    public interface IUserService
    {
        // --- MEMBER ---
        Task<ApiResponse<UserProfileDto>> GetMyProfileAsync(Guid userId);
        Task<ApiResponse<bool>> UpdateMyProfileAsync(Guid userId, UpdateProfileRequest request);

        // --- ADMIN ---
        Task<ApiResponse<UserDto>> GetUserByIdAsync(Guid userId);
        Task<ApiResponse<PagedResult<UserDto>>> GetAllUsersAsync(UserFilterRequest query);
        Task<ApiResponse<bool>> ToggleUserStatusAsync(Guid userId, bool isActive, string? reason);
    }
}