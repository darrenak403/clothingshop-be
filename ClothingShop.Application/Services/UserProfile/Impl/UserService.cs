using ClothingShop.Application.DTOs.User;
using ClothingShop.Application.Services.UserProfile.Interfaces;
using ClothingShop.Application.Wrapper;

namespace ClothingShop.Application.Services.UserProfile.Impl
{
    public class UserService : IUserService
    {
        public Task<ApiResponse<PagedResult<UserDto>>> GetAllUsersAsync(UserFilterRequest query)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<UserProfileDto>> GetMyProfileAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<UserDto>> GetUserByIdAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> ToggleUserStatusAsync(Guid userId, bool isActive, string? reason)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> UpdateMyProfileAsync(Guid userId, UpdateProfileRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
