using System.Net;
using ClothingShop.Application.DTOs.Address;
using ClothingShop.Application.Services.AddressService.Interfaces;
using ClothingShop.Application.Wrapper;
using ClothingShop.Domain.Entities;
using ClothingShop.Infrastructure.Interfaces;

namespace ClothingShop.Application.Services.AddressService.Impl
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddressService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //--- HELPER ---
        private async Task UnsetDefaultAddressForUser(Guid userId)
        {
            // Lấy tất cả địa chỉ của user
            var allAddresses = await _unitOfWork.Addresses.GetAllAsync();
            var userAddresses = allAddresses.Where(x => x.UserId == userId && x.IsDefault).ToList();

            foreach (var addr in userAddresses)
            {
                addr.IsDefault = false;
                await _unitOfWork.Addresses.UpdateAsync(addr);
            }
            // Lưu ý: Không gọi SaveChangesAsync ở đây để để hàm gọi chính (Caller) gọi 1 lần cho tối ưu transaction
        }

        public async Task<ApiResponse<IEnumerable<AddressDto>>> GetUserAddressesAsync(Guid userId)
        {
            var addresses = await _unitOfWork.Addresses.GetAllAsync();

            var userAddresses = addresses.Where(a => a.UserId == userId).ToList();

            var addressDtos = userAddresses.Select(a => new AddressDto
            {
                AddressId = a.Id,
                RecipientName = a.RecipientName,
                PhoneNumber = a.PhoneNumber,
                Street = a.Street,
                Ward = a.Ward,
                District = a.District,
                City = a.City,
                IsDefault = a.IsDefault
            });

            return ApiResponse<IEnumerable<AddressDto>>.SuccessResponse(addressDtos, "Lấy danh sách địa chỉ thành công");
        }

        public async Task<ApiResponse<AddressDto>> GetAddressByIdAsync(Guid userId, Guid addressId)
        {
            var address = await _unitOfWork.Addresses.GetByIdAsync(addressId);

            if (address == null)
                return ApiResponse<AddressDto>.FailureResponse("Địa chỉ không tồn tại", HttpStatusCode.NotFound);

            if (address.UserId != userId)
            {
                return ApiResponse<AddressDto>.FailureResponse("Địa chỉ không tồn tại", HttpStatusCode.NotFound);
            }

            var addressDto = new AddressDto
            {
                AddressId = address.Id,
                RecipientName = address.RecipientName,
                PhoneNumber = address.PhoneNumber,
                Street = address.Street,
                Ward = address.Ward,
                District = address.District,
                City = address.City,
                IsDefault = address.IsDefault
            };

            return ApiResponse<AddressDto>.SuccessResponse(addressDto, "Lấy thông tin địa chỉ thành công");
        }

        public async Task<ApiResponse<AddressDto>> CreateAddressAsync(Guid userId, CreateAddressRequest request)
        {
            // Logic: Nếu địa chỉ mới là Default, phải bỏ Default của các địa chỉ cũ
            if (request.IsDefault)
            {
                await UnsetDefaultAddressForUser(userId);
            }

            var newAddress = new Address
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RecipientName = request.RecipientName,
                PhoneNumber = request.PhoneNumber,
                Street = request.Street,
                Ward = request.Ward,
                District = request.District,
                City = request.City,
                IsDefault = request.IsDefault,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Addresses.AddAsync(newAddress);
            await _unitOfWork.SaveChangesAsync();

            var addressDto = new AddressDto
            {
                AddressId = newAddress.Id,
                RecipientName = newAddress.RecipientName,
                PhoneNumber = newAddress.PhoneNumber,
                Street = newAddress.Street,
                Ward = newAddress.Ward,
                District = newAddress.District,
                City = newAddress.City,
                IsDefault = newAddress.IsDefault
            };

            return ApiResponse<AddressDto>.SuccessResponse(addressDto, "Thêm địa chỉ mới thành công", HttpStatusCode.Created);
        }

        public async Task<ApiResponse<bool>> UpdateAddressAsync(Guid userId, Guid addressId, UpdateAddressRequest request)
        {
            var address = await _unitOfWork.Addresses.GetByIdAsync(addressId);

            if (address == null)
                return ApiResponse<bool>.FailureResponse("Địa chỉ không tồn tại", HttpStatusCode.NotFound);

            if (address.UserId != userId)
                return ApiResponse<bool>.FailureResponse("Bạn không có quyền cập nhật địa chỉ này", HttpStatusCode.Forbidden);

            // Logic: Nếu user muốn set địa chỉ này thành Default
            if (request.IsDefault && !address.IsDefault)
            {
                await UnsetDefaultAddressForUser(userId);
            }

            address.RecipientName = request.RecipientName;
            address.PhoneNumber = request.PhoneNumber;
            address.Street = request.Street;
            address.Ward = request.Ward;
            address.District = request.District;
            address.City = request.City;
            address.IsDefault = request.IsDefault;
            address.LastModifiedAt = DateTime.UtcNow;

            await _unitOfWork.Addresses.UpdateAsync(address);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Cập nhật địa chỉ thành công");
        }

        public async Task<ApiResponse<bool>> DeleteAddressAsync(Guid userId, Guid addressId)
        {
            var address = await _unitOfWork.Addresses.GetByIdAsync(addressId);

            if (address == null)
                return ApiResponse<bool>.FailureResponse("Địa chỉ không tồn tại", HttpStatusCode.NotFound);

            if (address.UserId != userId)
                return ApiResponse<bool>.FailureResponse("Bạn không có quyền cập nhật địa chỉ này", HttpStatusCode.Forbidden);


            _unitOfWork.Addresses.Delete(address);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Xóa địa chỉ thành công");
        }


        public async Task<ApiResponse<bool>> SetDefaultAddressAsync(Guid userId, Guid addressId)
        {
            var address = await _unitOfWork.Addresses.GetByIdAsync(addressId);

            if (address == null)
                return ApiResponse<bool>.FailureResponse("Địa chỉ không tồn tại", HttpStatusCode.NotFound);

            if (address.UserId != userId)
                return ApiResponse<bool>.FailureResponse("Bạn không có quyền cập nhật địa chỉ này", HttpStatusCode.Forbidden);

            await UnsetDefaultAddressForUser(userId);

            address.IsDefault = true;
            address.LastModifiedAt = DateTime.UtcNow;

            await _unitOfWork.Addresses.UpdateAsync(address);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Đặt địa chỉ mặc định thành công");

        }


    }
}
