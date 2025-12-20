using System.Net;
using ClothingShop.Application.DTOs.User;
using ClothingShop.Application.Services.UserProfile.Interfaces;
using ClothingShop.Application.Wrapper;
using ClothingShop.Infrastructure.Interfaces;

namespace ClothingShop.Application.Services.UserProfile.Impl
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // --- MEMBER ---
        public async Task<ApiResponse<UserProfileDto>> GetMyProfileAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<UserProfileDto>.FailureResponse("User not found", "NotFound", HttpStatusCode.NotFound);

            var role = await _unitOfWork.Roles.GetByIdAsync(user.RoleId);

            var userProfile = new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AvatarUrl = user.AvatarUrl,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                RoleName = role?.Name ?? "N/A"
            };

            return ApiResponse<UserProfileDto>.SuccessResponse(userProfile, "Lấy thông tin thành công", HttpStatusCode.OK);
        }

        public async Task<ApiResponse<bool>> UpdateMyProfileAsync(Guid userId, UpdateProfileRequest request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<bool>.FailureResponse("User not found", "NotFound", HttpStatusCode.NotFound);

            user.FullName = request.FullName;
            user.PhoneNumber = request.PhoneNumber;
            user.AvatarUrl = request.AvatarUrl;
            user.DateOfBirth = request.DateOfBirth;
            user.Gender = request.Gender;
            user.LastModifiedAt = DateTime.UtcNow;

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Cập nhật thông tin thành công", HttpStatusCode.OK);
        }

        // --- ADMIN ---
        public async Task<ApiResponse<PagedResult<UserDto>>> GetAllUsersAsync(UserFilterRequest query)
        {
            // Gọi Repository để lấy dữ liệu phân trang và filter
            // Lưu ý: Hàm GetPagedUsersAsync cần được implement trong UserRepository (như đã bàn ở các bước trước)
            var (users, totalCount) = await _unitOfWork.Users.GetPagedUsersAsync(
                query.Keyword,
                query.IsActive,
                query.PageNumber,
            query.PageSize
            );

            // Mapping List<User> sang List<UserDto>
            // Lưu ý: Để lấy RoleName trong list, tối ưu nhất là Repository nên .Include(u => u.Role)
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                AvatarUrl = u.AvatarUrl,
                DateOfBirth = u.DateOfBirth,
                Gender = u.Gender,
                RoleName = u.Role?.Name ?? "N/A",

                // Các trường Admin-only
                IsActive = u.IsActive,
                LockReason = u.LockReason,
                CreatedAt = u.CreatedAt,
                LastModifiedAt = u.LastModifiedAt,
                TotalOrders = u.Orders?.Count ?? 0
            }).ToList();

            var pagedResult = new PagedResult<UserDto>(userDtos, query.PageNumber, query.PageSize, totalCount);

            return ApiResponse<PagedResult<UserDto>>.SuccessResponse(pagedResult, "Lấy danh sách người dùng thành công", HttpStatusCode.OK);
        }


        public async Task<ApiResponse<UserDto>> GetUserByIdAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<UserDto>.FailureResponse("User not found", "NotFound", HttpStatusCode.NotFound);

            var role = await _unitOfWork.Roles.GetByIdAsync(user.RoleId);

            var userDto = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AvatarUrl = user.AvatarUrl,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                RoleName = role?.Name ?? "N/A",

                IsActive = user.IsActive,
                LockReason = user.LockReason,
                CreatedAt = user.CreatedAt,
                LastModifiedAt = user.LastModifiedAt,

                // Nếu User Entity đã config Navigation Property Orders, ta có thể đếm
                // Nếu chưa load, có thể cần gọi _unitOfWork.Orders.CountAsync(...) (Tuỳ Generic Repo)
                TotalOrders = user.Orders?.Count ?? 0
            };

            return ApiResponse<UserDto>.SuccessResponse(userDto, "Lấy thông tin người dùng thành công", HttpStatusCode.OK);
        }

        public async Task<ApiResponse<bool>> ToggleUserStatusAsync(Guid userId, bool isActive, string? reason)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<bool>.FailureResponse("User not found", "NotFound", HttpStatusCode.NotFound);

            user.IsActive = isActive;

            // Nếu khóa (isActive = false) thì lưu lý do, nếu mở khóa thì xóa lý do
            user.LockReason = isActive ? null : reason;
            user.LastModifiedAt = DateTime.UtcNow;

            // Nếu khóa tài khoản, có thể cần xóa RefreshToken để bắt user đăng xuất ngay lập tức
            if (!isActive)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiry = null;
            }

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var action = isActive ? "Mở khóa" : "Khóa";
            return ApiResponse<bool>.SuccessResponse(true, $"{action} tài khoản thành công");
        }

    }
}
