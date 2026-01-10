using ClothingShop.Application.DTOs.Brand;
using ClothingShop.Application.Wrapper;

namespace ClothingShop.Application.Services.BrandService.Interfaces
{
    public interface IBrandService
    {
        Task<ApiResponse<List<BrandDTO>>> GetAllBrandsAsync();
        Task<ApiResponse<BrandDTO>> GetBrandByIdAsync(Guid id);
        Task<ApiResponse<BrandDTO>> CreateBrandAsync(BrandCreateRequest request);
        Task<ApiResponse<BrandDTO>> UpdateBrandAsync(Guid id, BrandCreateRequest request);
        Task<ApiResponse<bool>> DeleteBrandAsync(Guid id);
    }
}
