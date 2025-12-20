using ClothingShop.Application.DTOs.User;
using ClothingShop.Application.Wrapper;
using Microsoft.AspNetCore.Http;

namespace ClothingShop.Application.Services.UserProfile.Interfaces
{
    public interface IUserService
    {
        // --- MEMBER ---
        Task<ApiResponse<UserProfileDto>> GetMyProfileAsync(Guid userId);
        Task<ApiResponse<bool>> UpdateMyProfileAsync(Guid userId, UpdateProfileRequest request);
        Task<ApiResponse<bool>> UpdateAvatarProfileAsync(Guid userId, IFormFile file);
        // --- ADMIN ---
        Task<ApiResponse<UserDto>> GetUserByIdAsync(Guid userId);
        Task<ApiResponse<PagedResult<UserDto>>> GetAllUsersAsync(UserFilterRequest query);
        Task<ApiResponse<bool>> ToggleUserStatusAsync(Guid userId, bool isActive, string? reason);
    }
}