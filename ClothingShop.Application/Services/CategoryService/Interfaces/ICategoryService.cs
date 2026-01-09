using ClothingShop.Application.DTOs.Category;
using ClothingShop.Application.Wrapper;

namespace ClothingShop.Application.Services.CategoryService.Interfaces
{
    public interface ICategoryService
    {
        Task<ApiResponse<List<CategoryDTO>>> GetAllCategoriesRecursive();
        Task<ApiResponse<CategoryDTO>> GetCategoryByIdAsync(Guid id);
        Task<ApiResponse<bool>> CreateCategoryAsync(CategoryCreateRequest request);
        Task<ApiResponse<bool>> UpdateCategoryAsync(Guid id, CategoryCreateRequest request);
        Task<ApiResponse<bool>> DeleteCategoryAsync(Guid id);
    }
}
