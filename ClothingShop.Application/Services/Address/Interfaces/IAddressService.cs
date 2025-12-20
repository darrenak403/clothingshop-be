using ClothingShop.Application.DTOs.Address;

namespace ClothingShop.Application.Services.Address.Interfaces
{

    public interface IAddressService
    {
        Task<IEnumerable<AddressDto>> GetUserAddressesAsync(Guid userId);
        Task<AddressDto> GetAddressByIdAsync(Guid addressId);
        Task<AddressDto> CreateAddressAsync(Guid userId, CreateAddressRequest request);
        Task<bool> UpdateAddressAsync(Guid userId, Guid addressId, UpdateAddressRequest request);
        Task<bool> DeleteAddressAsync(Guid userId, Guid addressId);

        // Logic quan trọng: Đặt một địa chỉ làm mặc định
        Task<bool> SetDefaultAddressAsync(Guid userId, Guid addressId);
    }
}
