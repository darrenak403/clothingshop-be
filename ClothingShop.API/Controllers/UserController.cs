using ClothingShop.Application.DTOs.User;
using ClothingShop.Application.Services.AddressService.Interfaces;
using ClothingShop.Application.Services.Auth.Interfaces;
using ClothingShop.Application.Services.UserProfile.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClothingShop.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAddressService _addressService;
        private readonly ICurrentUserService _currentUserService;
        public UserController(IUserService userService, IAddressService addressService, ICurrentUserService currentUserService)
        {
            _userService = userService;
            _addressService = addressService;
            _currentUserService = currentUserService;
        }

        private Guid CurrentUserId => _currentUserService.UserId
                                      ?? throw new UnauthorizedAccessException("User ID is missing.");

        // ============================================
        // GET: api/users/profile
        // Lấy thông tin profile của user hiện tại
        // ============================================
        [HttpGet("profile")]
        public async Task<IActionResult> GetMyProfileAsync()
        {
            var response = await _userService.GetMyProfileAsync(CurrentUserId);
            return StatusCode(response.Status, response);
        }

        // ============================================
        // PATCH: api/users/profile
        // Cập nhật profile của user hiện tại
        // ============================================
        [HttpPatch("profile")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileRequest request)
        {
            var response = await _userService.UpdateMyProfileAsync(CurrentUserId, request);
            return StatusCode(response.Status, response);
        }

        // ============================================
        // POST: api/users/avatar
        // Upload avatar cho user hiện tại
        // ============================================
        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            var result = await _userService.UpdateAvatarProfileAsync(CurrentUserId, file);
            return StatusCode(result.Status, result);
        }

        // ============================================
        // GET: api/users
        // Lấy danh sách tất cả user (Admin only)
        // ============================================
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsersAsync([FromQuery] UserFilterRequest query)
        {
            var response = await _userService.GetAllUsersAsync(query);
            return StatusCode(response.Status, response);
        }

        // ============================================
        // GET: api/users/{id}
        // Lấy thông tin user theo ID (Admin only)
        // ============================================
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserByIdAsync([FromRoute] Guid id)
        {
            var response = await _userService.GetUserByIdAsync(id);
            return StatusCode(response.Status, response);
        }

        // ============================================
        // PATCH: api/users/{id}/status
        // Khóa/Mở khóa tài khoản user (Admin only)
        // ============================================
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleUserStatus(Guid id, [FromBody] ToggleUserStatusRequest request)
        {
            // request.IsActive: true (mở), false (khóa)
            // request.Reason: lý do khóa
            var response = await _userService.ToggleUserStatusAsync(id, request.IsActive, request.Reason);
            return StatusCode(response.Status, response);
        }
    }
}