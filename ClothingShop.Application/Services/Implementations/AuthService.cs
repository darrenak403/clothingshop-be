using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ClothingShop.Application.DTOs.Auth;
using ClothingShop.Application.DTOs.User;
using ClothingShop.Application.Interfaces;
using ClothingShop.Application.Services.Interfaces;
using ClothingShop.Application.Wrapper;
using ClothingShop.Domain.Entities;
using ClothingShop.Domain.Enums;
using ClothingShop.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ClothingShop.Application.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<User> _userRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IPasswordResetHistoryRepository _passwordResetHistoryRepo;

        public AuthService(
            IGenericRepository<User> userRepo,
            IRoleRepository roleRepo,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IEmailService emailService,
            IPasswordResetHistoryRepository passwordResetHistoryRepo)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _emailService = emailService;
            _passwordResetHistoryRepo = passwordResetHistoryRepo;
        }

        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
        {
            var existingUser = await _userRepo.FindAsync(u => u.Email == request.Email);
            if (existingUser == null)
                return ApiResponse<LoginResponse>.FailureResponse("Invalid email or password.", "Unauthorized", HttpStatusCode.Unauthorized);

            var isPasswordValid = _passwordHasher.VerifyPassword(existingUser.PasswordHash, request.Password);
            if (!isPasswordValid)
                return ApiResponse<LoginResponse>.FailureResponse("Invalid email or password.", "Unauthorized", HttpStatusCode.Unauthorized);

            if (!existingUser.IsActive)
                return ApiResponse<LoginResponse>.FailureResponse("Account is locked.", "Forbidden", HttpStatusCode.Forbidden);

            var loginResponse = await GenerateAndSaveTokensAsync(existingUser);
            return ApiResponse<LoginResponse>.SuccessResponse(loginResponse, "Login successful", HttpStatusCode.OK);
        }

        private async Task<LoginResponse> GenerateAndSaveTokensAsync(User existingUser)
        {
            var role = await _roleRepo.GetByIdAsync(existingUser.RoleId);
            var accessToken = GenerateJwtToken(existingUser, role!.Name);
            var refreshToken = GenerateRefreshToken();

            existingUser.RefreshToken = refreshToken;
            existingUser.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            await _userRepo.UpdateAsync(existingUser);
            await _unitOfWork.SaveChangesAsync();

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = new UserDto
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
            var existingUser = await _userRepo.FindAsync(u => u.Email == request.Email);
            if (existingUser != null)
                return ApiResponse<RegisterResponse>.FailureResponse("Email is already registered.", "Conflict", HttpStatusCode.Conflict);

            var role = await _roleRepo.GetByNameAsync("Customer");
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

            await _userRepo.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var registerResponse = new RegisterResponse
            {
                User = new UserDto
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
            var user = await _userRepo.FindAsync(u => u.RefreshToken == refreshToken);
            if (user == null || user.RefreshTokenExpiry == null || user.RefreshTokenExpiry <= DateTime.UtcNow)
                return ApiResponse<LoginResponse>.FailureResponse("Invalid or expired refresh token.", "Unauthorized", HttpStatusCode.Unauthorized);

            var result = await GenerateAndSaveTokensAsync(user);
            return ApiResponse<LoginResponse>.SuccessResponse(result, "Token refreshed", HttpStatusCode.OK);
        }

        public async Task<ApiResponse<string>> LogoutAsync(string refreshToken)
        {
            var user = await _userRepo.FindAsync(u => u.RefreshToken == refreshToken);
            if (user == null)
                return ApiResponse<string>.FailureResponse("Invalid refresh token.", "Unauthorized", HttpStatusCode.Unauthorized);

            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            await _userRepo.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse(string.Empty, "Logged out successfully", HttpStatusCode.OK);
        }

        public async Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var user = await _userRepo.FindAsync(u => u.Email == request.Email);
            if (user == null)
                return ApiResponse<string>.FailureResponse("Email not found.", "NotFound", HttpStatusCode.NotFound);

            // Check rate limiting: Max 5 requests per day
            var todayCount = await _passwordResetHistoryRepo.GetTodayRequestCountAsync(user.Id);
            if (todayCount >= 5)
                return ApiResponse<string>.FailureResponse("Quá nhiều yêu cầu. Vui lòng thử lại sau 24 giờ.", "TooManyRequests", HttpStatusCode.TooManyRequests);

            // Mark all pending OTPs as expired
            await _passwordResetHistoryRepo.MarkAllAsExpiredAsync(user.Id);

            // Generate new OTP
            var otp = GenerateOtp();
            var otpExpiryMinutes = int.Parse(_configuration["Auth:OtpExpiryMinutes"] ?? "15");

            var resetHistory = new PasswordResetHistory
            {
                UserId = user.Id,
                Otp = otp,
                OtpGeneratedAt = DateTime.UtcNow,
                OtpExpiresAt = DateTime.UtcNow.AddMinutes(otpExpiryMinutes),
                Status = AttemptStatus.Pending
            };

            await _passwordResetHistoryRepo.AddAsync(resetHistory);
            await _unitOfWork.SaveChangesAsync();

            // Send OTP via email
            var emailSubject = "Mã OTP Đặt Lại Mật Khẩu - ClothingShop";
            var emailBody = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #2196F3; color: white; padding: 20px; text-align: center; }}
                        .content {{ background-color: #f9f9f9; padding: 30px; border-radius: 5px; margin-top: 20px; }}
                        .otp-code {{ background-color: #fff; border: 2px dashed #2196F3; padding: 15px; text-align: center; font-size: 32px; font-weight: bold; color: #2196F3; letter-spacing: 5px; margin: 20px 0; }}
                        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #777; }}
                        .warning {{ color: #f44336; font-weight: bold; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>ClothingShop</h1>
                        </div>
                        <div class='content'>
                            <h2>Xin chào {user.FullName},</h2>
                            <p>Bạn đã yêu cầu đặt lại mật khẩu cho tài khoản của mình.</p>
                            <p>Mã OTP của bạn là:</p>
                            <div class='otp-code'>{otp}</div>
                            <p><strong>Mã này có hiệu lực trong {otpExpiryMinutes} phút.</strong></p>
                            <p class='warning'>⚠️ Không chia sẻ mã này với bất kỳ ai!</p>
                            <p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này hoặc liên hệ với chúng tôi ngay.</p>
                        </div>
                        <div class='footer'>
                            <p>© 2025 ClothingShop. All rights reserved.</p>
                            <p>Email này được gửi tự động, vui lòng không trả lời.</p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            try
            {
                await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);
                return ApiResponse<string>.SuccessResponse(string.Empty, "OTP đã được gửi đến email của bạn", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                resetHistory.Status = AttemptStatus.Failed;
                await _passwordResetHistoryRepo.UpdateAsync(resetHistory);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<string>.FailureResponse($"Không thể gửi email: {ex.Message}", "Email Error", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userRepo.FindAsync(u => u.Email == request.Email);
            if (user == null)
                return ApiResponse<string>.FailureResponse("Email not found.", "NotFound", HttpStatusCode.NotFound);

            // Get latest valid OTP
            var resetHistory = await _passwordResetHistoryRepo.GetLatestValidOtpAsync(user.Id, request.Otp);

            if (resetHistory == null)
            {
                // Log failed attempt
                var allPending = await _passwordResetHistoryRepo
                    .GetAllUsedOtpAsync(p => p.UserId == user.Id && p.Otp == request.Otp && !p.IsUsed);

                if (allPending.Any())
                {
                    var latest = allPending.OrderByDescending(p => p.OtpGeneratedAt).First();
                    latest.AttemptCount++;

                    if (latest.AttemptCount >= 5)
                    {
                        latest.Status = AttemptStatus.Failed;
                    }

                    await _passwordResetHistoryRepo.UpdateAsync(latest);
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

            await _userRepo.UpdateAsync(user);
            await _passwordResetHistoryRepo.UpdateAsync(resetHistory);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse(string.Empty, "Mật khẩu đã được đặt lại thành công", HttpStatusCode.OK);
        }

        public async Task<ApiResponse<string>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<string>.FailureResponse("User not found.", "NotFound", HttpStatusCode.NotFound);

            var isValidPassword = _passwordHasher.VerifyPassword(user.PasswordHash, request.CurrentPassword);
            if (!isValidPassword)
                return ApiResponse<string>.FailureResponse("Mật khẩu hiện tại không chính xác.", "Unauthorized", HttpStatusCode.Unauthorized);

            user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;

            await _userRepo.UpdateAsync(user);
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
