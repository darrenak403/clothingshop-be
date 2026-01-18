using System.Net;
using ClothingShop.Application.DTOs.User;
using ClothingShop.Application.Services.UserProfile.Interfaces;
using ClothingShop.Application.Wrapper;
using ClothingShop.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ClothingShop.Application.Services.UserProfile.Impl
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPhotoService _photoService;
        public UserService(IUnitOfWork unitOfWork, IPhotoService photoService)
        {
            _unitOfWork = unitOfWork;
            _photoService = photoService;
        }

        // --- MEMBER ---
        public async Task<ApiResponse<UserProfileDto>> GetMyProfileAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<UserProfileDto>.FailureResponse("Không tìm thấy người dùng", HttpStatusCode.NotFound);

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

            return ApiResponse<UserProfileDto>.SuccessResponse(userProfile, "Lấy thông tin thành công");
        }

        public async Task<ApiResponse<bool>> UpdateMyProfileAsync(Guid userId, UpdateProfileRequest request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<bool>.FailureResponse("Không tìm thấy người dùng", HttpStatusCode.NotFound);


            // 1. FullName: Nếu request có gửi chuỗi (không rỗng/null) thì mới update
            if (!string.IsNullOrEmpty(request.FullName))
            {
                user.FullName = request.FullName;
            }

            // 2. PhoneNumber: Nếu request khác null thì update
            // Lưu ý: Nếu user muốn xóa SĐT, FE cần gửi chuỗi rỗng "" (tùy quy ước của bạn)
            if (request.PhoneNumber != null)
            {
                user.PhoneNumber = request.PhoneNumber;
            }

            // 3. DateOfBirth (Kiểu DateTime?)
            // Chỉ update nếu có giá trị
            if (request.DateOfBirth.HasValue)
            {
                user.DateOfBirth = request.DateOfBirth.Value;
            }

            // 4. Gender
            if (!string.IsNullOrEmpty(request.Gender))
            {
                user.Gender = request.Gender;
            }

            user.LastModifiedAt = DateTime.UtcNow;

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Cập nhật hồ sơ thành công");
        }


        public async Task<ApiResponse<bool>> UpdateAvatarProfileAsync(Guid userId, IFormFile file)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<bool>.FailureResponse("Không tìm thấy người dùng", HttpStatusCode.NotFound);

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null)
                return ApiResponse<bool>.FailureResponse("Lỗi khi tải ảnh lên Cloudinary: " + result.Error.Message, HttpStatusCode.InternalServerError);

            // 3. (Tùy chọn) Xóa ảnh cũ trên Cloudinary nếu cần thiết
            // if (!string.IsNullOrEmpty(user.AvatarUrl)) { ... logic xóa ... }

            // 4. Lưu URL mới vào Database
            user.AvatarUrl = result.SecureUrl.AbsoluteUri;

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // 5. Trả về URL mới để Frontend hiển thị
            return ApiResponse<bool>.SuccessResponse(true, "Cập nhật ảnh đại diện thành công");
        }

        // --- ADMIN ---
        public async Task<ApiResponse<PagedResult<UserDto>>> GetAllUsersAsync(UserFilterRequest query)
        {
            // Gọi Repository để lấy dữ liệu phân trang và filter
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

            return ApiResponse<UserDto>.SuccessPagedResponse(
                userDtos,
                totalCount,
                query.PageNumber,
                query.PageSize,
                "Lấy danh sách người dùng thành công");
        }


        public async Task<ApiResponse<UserDto>> GetUserByIdAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<UserDto>.FailureResponse("Không tìm thấy người dùng", HttpStatusCode.NotFound);

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

            return ApiResponse<UserDto>.SuccessResponse(userDto, "Lấy thông tin người dùng thành công");
        }

        public async Task<ApiResponse<bool>> ToggleUserStatusAsync(Guid userId, bool isActive, string? reason)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<bool>.FailureResponse("Không tìm thấy người dùng", HttpStatusCode.NotFound);

            if (user.IsActive == isActive)
                return ApiResponse<bool>.FailureResponse("Trạng thái tài khoản không thay đổi", HttpStatusCode.BadRequest);

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
