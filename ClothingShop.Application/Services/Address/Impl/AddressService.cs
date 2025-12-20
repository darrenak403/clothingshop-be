using ClothingShop.Application.DTOs.Address;
using ClothingShop.Application.Services.Address.Interfaces;
using ClothingShop.Application.Wrapper;

namespace ClothingShop.Application.Services.Address.Impl
{
    public class AddressService : IAddressService
    {
        public Task<ApiResponse<AddressDto>> CreateAddressAsync(Guid userId, CreateAddressRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> DeleteAddressAsync(Guid userId, Guid addressId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<AddressDto>> GetAddressByIdAsync(Guid addressId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<IEnumerable<AddressDto>>> GetUserAddressesAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> SetDefaultAddressAsync(Guid userId, Guid addressId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> UpdateAddressAsync(Guid userId, Guid addressId, UpdateAddressRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
