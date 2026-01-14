using ClothingShop.Application.DTOs.Auth;
using ClothingShop.Application.Services.Auth.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClothingShop.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // ============================================
        // POST: api/auth/register
        // Đăng ký tài khoản mới (Public)
        // ============================================
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authService.RegisterAsync(request);
            if (!response.Success)
                return BadRequest(response);
            return StatusCode(response.Status, response);
        }

        // ============================================
        // POST: api/auth/login
        // Đăng nhập (Public)
        // ============================================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _authService.LoginAsync(request);
            if (!response.Success)
                return Unauthorized(response);
            return Ok(response);
        }

        // ============================================
        // POST: api/auth/refresh-token
        // Làm mới access token (Public)
        // ============================================
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _authService.RefreshTokenAsync(request.RefreshToken);
            if (!response.Success)
                return Unauthorized(response);
            return Ok(response);
        }

        // ============================================
        // POST: api/auth/logout
        // Đăng xuất và thu hồi token
        // ============================================
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authService.LogoutAsync(request.RefreshToken);

            if (!response.Success)
            {
                return StatusCode((int)response.Status, response);
            }

            return Ok(response);
        }

        // ============================================
        // POST: api/auth/forgot-password
        // Gửi email reset mật khẩu (Public)
        // ============================================
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _authService.ForgotPasswordAsync(request);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }

        // ============================================
        // POST: api/auth/change-password
        // Đặt mật khẩu mới từ email reset (Public)
        // ============================================
        [HttpPost("change-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _authService.ResetPasswordAsync(request);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }
    }
}
