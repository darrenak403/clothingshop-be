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

        public AuthService(
            IGenericRepository<User> userRepo,
            IRoleRepository roleRepo,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
        {
            var existingUser = await _userRepo.FindAsync(u => u.Email == request.Email);
            if (existingUser == null)
                return ApiResponse<LoginResponse>.FailureResponse("Invalid email or password.", "Unauthorized", HttpStatusCode.Unauthorized);

            var isPasswordValid = _passwordHasher.VerifyPassword(existingUser.PasswordHash, request.Password);
            if (!isPasswordValid)
                return ApiResponse<LoginResponse>.FailureResponse("Invalid email or password.", "Unauthorized", HttpStatusCode.Unauthorized);

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
                    Id = existingUser.Id,
                    FullName = existingUser.FullName,
                    Email = existingUser.Email,
                    PhoneNumber = existingUser.PhoneNumber,
                    RoleName = role!.Name
                }
            };
        }

        private string GenerateJwtToken(User user, string roleName)
        {
            var key = _configuration["Jwt:Key"];
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
                    Id = user.Id,
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
    }
}
