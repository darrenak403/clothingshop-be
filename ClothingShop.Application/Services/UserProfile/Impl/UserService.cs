using ClothingShop.Application.DTOs.User;
using ClothingShop.Application.Services.UserProfile.Interfaces;
using ClothingShop.Application.Wrapper;

namespace ClothingShop.Application.Services.UserProfile.Impl
{
    public class UserService : IUserService
    {
        public Task<PagedResult<UserDto>> GetAllUsersAsync(UserFilterRequest query)
        {
            throw new NotImplementedException();
        }

        public Task<UserProfileDto> GetMyProfileAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> GetUserByIdAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ToggleUserStatusAsync(Guid userId, bool isActive, string? reason)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateMyProfileAsync(Guid userId, UpdateProfileRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
