using ClothingShop.Application.DTOs.Address;
using ClothingShop.Application.Services.Address.Interfaces;

namespace ClothingShop.Application.Services.Address.Impl
{
    public class AddressService : IAddressService
    {
        public Task<AddressDto> CreateAddressAsync(Guid userId, CreateAddressRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAddressAsync(Guid userId, Guid addressId)
        {
            throw new NotImplementedException();
        }

        public Task<AddressDto> GetAddressByIdAsync(Guid addressId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AddressDto>> GetUserAddressesAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetDefaultAddressAsync(Guid userId, Guid addressId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAddressAsync(Guid userId, Guid addressId, UpdateAddressRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
