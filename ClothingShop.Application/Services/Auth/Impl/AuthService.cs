using ClothingShop.Application.DTOs.Auth;
using ClothingShop.Application.DTOs.User;
using ClothingShop.Application.Services.Auth.Interfaces;
using ClothingShop.Application.Wrapper;
using ClothingShop.Domain.Entities;
using ClothingShop.Domain.Enums;
using ClothingShop.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ClothingShop.Application.Services.Auth.Impl
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IConfiguration configuration,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
        {
            var existingUser = await _unitOfWork.Users.FindAsync(u => u.Email == request.Email);
            if (existingUser == null)
                return ApiResponse<LoginResponse>.FailureResponse("Người dùng không tồn tại.", "Unauthorized", HttpStatusCode.Unauthorized);

            var isPasswordValid = _passwordHasher.VerifyPassword(existingUser.PasswordHash, request.Password);
            if (!isPasswordValid)
                return ApiResponse<LoginResponse>.FailureResponse("Mật khẩu không chính xác", "Unauthorized", HttpStatusCode.Unauthorized);

            if (!existingUser.IsActive)
                return ApiResponse<LoginResponse>.FailureResponse("Tài khoản của bạn đã bị khóa!.", "Forbidden", HttpStatusCode.Forbidden);

            var loginResponse = await GenerateAndSaveTokensAsync(existingUser);
            return ApiResponse<LoginResponse>.SuccessResponse(loginResponse, "Login successful", HttpStatusCode.OK);
        }

        private async Task<LoginResponse> GenerateAndSaveTokensAsync(User existingUser)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(existingUser.RoleId);
            var accessToken = GenerateJwtToken(existingUser, role!.Name);
            var refreshToken = GenerateRefreshToken();

            existingUser.RefreshToken = refreshToken;
            existingUser.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            await _unitOfWork.Users.UpdateAsync(existingUser);
            // Lưu tất cả thay đổi vào DB trong 1 transaction
            await _unitOfWork.SaveChangesAsync();

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = new UserAuthDto
                {
                    UserId = existingUser.Id,
                    FullName = existingUser.FullName,
                    Email = existingUser.Email,
                    PhoneNumber = existingUser.PhoneNumber,
                    RoleName = role!.Name
                }
            };
        }

        private string GenerateJwtToken(User user, string roleName)
        {
            var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured in appsettings.json");
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expiryMinutes = int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, roleName)
            };


            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }

        public async Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _unitOfWork.Users.FindAsync(u => u.Email == request.Email);
            if (existingUser != null)
                return ApiResponse<RegisterResponse>.FailureResponse("Email is already registered.", "Conflict", HttpStatusCode.Conflict);

            var role = await _unitOfWork.Roles.GetByNameAsync("Customer");
            if (role == null)
                return ApiResponse<RegisterResponse>.FailureResponse("Default role not found.", "Server error", HttpStatusCode.InternalServerError);

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                RoleId = role.Id
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var registerResponse = new RegisterResponse
            {
                User = new UserAuthDto
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    RoleName = role.Name
                }
            };

            return ApiResponse<RegisterResponse>.SuccessResponse(registerResponse, "Registered", HttpStatusCode.Created);
        }

        public async Task<ApiResponse<LoginResponse>> RefreshTokenAsync(string refreshToken)
        {
            var user = await _unitOfWork.Users.FindAsync(u => u.RefreshToken == refreshToken);
            if (user == null || user.RefreshTokenExpiry == null || user.RefreshTokenExpiry <= DateTime.UtcNow)
                return ApiResponse<LoginResponse>.FailureResponse("Invalid or expired refresh token.", "Unauthorized", HttpStatusCode.Unauthorized);

            var result = await GenerateAndSaveTokensAsync(user);
            return ApiResponse<LoginResponse>.SuccessResponse(result, "Token refreshed", HttpStatusCode.OK);
        }

        public async Task<ApiResponse<string>> LogoutAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                return ApiResponse<string>.FailureResponse("Refresh token is required.", "Bad Request", HttpStatusCode.BadRequest);
            }

            var user = await _unitOfWork.Users.FindAsync(u => u.RefreshToken == refreshToken);

            if (user == null)
            {
                return ApiResponse<string>.FailureResponse("Refresh token not found or already revoked.", "Unauthorized", HttpStatusCode.Unauthorized);
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;

            await _unitOfWork.Users.UpdateAsync(user);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result > 0)
            {
                return ApiResponse<string>.SuccessResponse(string.Empty, "Logged out successfully.", HttpStatusCode.OK);
            }

            return ApiResponse<string>.FailureResponse("Could not save changes.", "Internal Server Error", HttpStatusCode.InternalServerError);
        }

        public async Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var user = await _unitOfWork.Users.FindAsync(u => u.Email == request.Email);
            if (user == null)
                return ApiResponse<string>.FailureResponse("Email not found.", "NotFound", HttpStatusCode.NotFound);

            // 1. Check rate limiting: Max 5 requests per day
            var todayCount = await _unitOfWork.PasswordResets.GetTodayRequestCountAsync(user.Id);
            if (todayCount >= 5)
                return ApiResponse<string>.FailureResponse("Quá nhiều yêu cầu. Vui lòng thử lại sau 24 giờ.", "TooManyRequests", HttpStatusCode.TooManyRequests);

            // 2. Generate OTP settings
            var otp = GenerateOtp();
            var otpExpiryMinutes = int.Parse(_configuration["Auth:OtpExpiryMinutes"] ?? "5");

            var existingHistory = await _unitOfWork.PasswordResets.GetByIdAsync(user.Id);

            PasswordResetHistory historyToEmail; // Biến tạm để dùng gửi email

            if (existingHistory != null)
            {
                existingHistory.Otp = otp;
                existingHistory.OtpGeneratedAt = DateTime.UtcNow;
                existingHistory.OtpExpiresAt = DateTime.UtcNow.AddMinutes(otpExpiryMinutes);
                existingHistory.Status = AttemptStatus.Pending;
                existingHistory.IsUsed = false;
                existingHistory.AttemptCount = 0; // Reset số lần thử sai

                await _unitOfWork.PasswordResets.UpdateAsync(existingHistory);
                historyToEmail = existingHistory;
            }
            else
            {
                var newHistory = new PasswordResetHistory
                {
                    UserId = user.Id,
                    Otp = otp,
                    OtpGeneratedAt = DateTime.UtcNow,
                    OtpExpiresAt = DateTime.UtcNow.AddMinutes(otpExpiryMinutes),
                    Status = AttemptStatus.Pending,
                    IsUsed = false,
                    AttemptCount = 0
                };

                await _unitOfWork.PasswordResets.AddAsync(newHistory);
                historyToEmail = newHistory;
            }

            await _unitOfWork.SaveChangesAsync();

            try
            {
                await _emailService.SendOtpEmailAsync(user.Email, user.FullName, otp, otpExpiryMinutes);
                return ApiResponse<string>.SuccessResponse(string.Empty, "OTP đã được gửi đến email của bạn", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                // Nếu gửi mail lỗi, cập nhật trạng thái Failed
                historyToEmail.Status = AttemptStatus.Failed;
                await _unitOfWork.PasswordResets.UpdateAsync(historyToEmail);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<string>.FailureResponse($"Không thể gửi email: {ex.Message}", "Email Error", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _unitOfWork.Users.FindAsync(u => u.Email == request.Email);
            if (user == null)
                return ApiResponse<string>.FailureResponse("Email not found.", "NotFound", HttpStatusCode.NotFound);

            // Get latest valid OTP
            var resetHistory = await _unitOfWork.PasswordResets.GetLatestValidOtpAsync(user.Id, request.Otp);

            if (resetHistory == null)
            {
                // Log failed attempt
                var allPending = await _unitOfWork.PasswordResets
                    .GetAllUsedOtpAsync(p => p.UserId == user.Id && p.Otp == request.Otp && !p.IsUsed);

                if (allPending.Any())
                {
                    var latest = allPending.OrderByDescending(p => p.OtpGeneratedAt).First();
                    latest.AttemptCount++;

                    if (latest.AttemptCount >= 5)
                    {
                        latest.Status = AttemptStatus.Failed;
                    }

                    await _unitOfWork.PasswordResets.UpdateAsync(latest);
                    await _unitOfWork.SaveChangesAsync();
                }

                return ApiResponse<string>.FailureResponse("OTP không hợp lệ hoặc đã hết hạn.", "Unauthorized", HttpStatusCode.Unauthorized);
            }

            // Reset password
            user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;

            // Mark OTP as used
            resetHistory.IsUsed = true;
            resetHistory.UsedAt = DateTime.UtcNow;
            resetHistory.Status = AttemptStatus.Used;

            // Cập nhật cả user và password reset history trong cùng 1 transaction
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.PasswordResets.UpdateAsync(resetHistory);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse(string.Empty, "Mật khẩu đã được đặt lại thành công", HttpStatusCode.OK);
        }

        public async Task<ApiResponse<string>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<string>.FailureResponse("Không tìm thấy người dùng.", "NotFound", HttpStatusCode.NotFound);

            var isValidPassword = _passwordHasher.VerifyPassword(user.PasswordHash, request.CurrentPassword);
            if (!isValidPassword)
                return ApiResponse<string>.FailureResponse("Mật khẩu hiện tại không chính xác.", "Unauthorized", HttpStatusCode.Unauthorized);

            user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse(string.Empty, "Mật khẩu đã được thay đổi thành công", HttpStatusCode.OK);
        }

        private string GenerateOtp(int length = 6)
        {
            var random = new Random();
            var otp = string.Empty;
            for (int i = 0; i < length; i++)
            {
                otp += random.Next(0, 10).ToString();
            }
            return otp;
        }
    }
}
