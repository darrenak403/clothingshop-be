using ClothingShop.Application.DTOs.Address;
using ClothingShop.Application.Wrapper;

namespace ClothingShop.Application.Services.AddressService.Interfaces
{
    public interface IAddressService
    {
        Task<ApiResponse<IEnumerable<AddressDto>>> GetUserAddressesAsync(Guid userId);
        Task<ApiResponse<AddressDto>> GetAddressByIdAsync(Guid userId, Guid addressId);
        Task<ApiResponse<AddressDto>> CreateAddressAsync(Guid userId, CreateAddressRequest request);
        Task<ApiResponse<bool>> UpdateAddressAsync(Guid userId, Guid addressId, UpdateAddressRequest request);
        Task<ApiResponse<bool>> DeleteAddressAsync(Guid userId, Guid addressId);
        Task<ApiResponse<bool>> SetDefaultAddressAsync(Guid userId, Guid addressId);
    }
}