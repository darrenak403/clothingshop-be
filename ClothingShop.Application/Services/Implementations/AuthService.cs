using ClothingShop.Application.DTOs.Auth;
using ClothingShop.Application.Services.Interfaces;
using ClothingShop.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ClothingShop.Application.Services.Implementations
{
    public class AuthService : IAuthService
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public Task<string> LoginAsync(LoginDto request)
        {
            throw new NotImplementedException();
        }

        public Task<string> RegisterAsync(RegisterDto request)
        {
            throw new NotImplementedException();
        }
    }
}
