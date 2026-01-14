using ClothingShop.Application.DTOs.Product;
using ClothingShop.Application.Wrapper;

namespace ClothingShop.Application.Services.ProductService.Interfaces
{
    public interface IProductService
    {
        Task<ApiResponse<List<ProductDTO>>> GetAllProductsAsync();
        Task<ApiResponse<ProductDTO>> GetProductByIdAsync(Guid id);
        Task<ApiResponse<ProductDTO>> GetProductBySlugAsync(string slug);
        Task<ApiResponse<ProductDTO>> CreateProductAsync(ProductCreateRequest request);
        Task<ApiResponse<ProductDTO>> UpdateProductAsync(Guid id, ProductUpdateRequest request);
        Task<ApiResponse<bool>> DeleteProductAsync(Guid id);

        // Custom methods
        Task<ApiResponse<List<ProductDTO>>> GetFeaturedProductsAsync();
        Task<ApiResponse<List<ProductDTO>>> GetProductsByCategoryAsync(Guid categoryId);
        Task<ApiResponse<List<ProductDTO>>> GetProductsByBrandAsync(Guid brandId);
    }
}
