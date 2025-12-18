using ClothingShop.Application.DTOs.User;

namespace ClothingShop.Application.Services.UserProfile.Interfaces
{
    public interface IUserService
    {
        // --- Dành cho Member (Cá nhân User tự thao tác) ---
        Task<UserProfileDto> GetMyProfileAsync(Guid userId);
        Task<bool> UpdateMyProfileAsync(Guid userId, UpdateProfileRequest request);

        // --- Dành cho Admin (Quản trị hệ thống) ---
        Task<UserDto> GetUserByIdAsync(Guid userId);
        Task<PagedResult<UserDto>> GetAllUsersAsync(UserFilterRequest query);
        Task<bool> ToggleUserStatusAsync(Guid userId, bool isActive, string? reason);
    }
}
