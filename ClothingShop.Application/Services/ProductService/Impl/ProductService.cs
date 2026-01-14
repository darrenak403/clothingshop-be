using ClothingShop.Application.DTOs.Product;
using ClothingShop.Application.Services.ProductService.Interfaces;
using ClothingShop.Application.Wrapper;
using ClothingShop.Domain.Entities;
using ClothingShop.Infrastructure.Interfaces;
using System.Text.RegularExpressions;

namespace ClothingShop.Application.Services.ProductService.Impl
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<ProductDTO>> CreateProductAsync(ProductCreateRequest request)
        {
            try
            {
                //B1: validate CategoryId
                var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
                if (category == null)
                {
                    return ApiResponse<ProductDTO>.FailureResponse("Danh mục không tồn tại", "Lỗi khi tạo sản phẩm");
                }

                //B2: validate BrandId (nếu có)
                if (request.BrandId.HasValue)
                {
                    var brand = await _unitOfWork.Brands.GetByIdAsync(request.BrandId.Value);
                    if (brand == null)
                    {
                        return ApiResponse<ProductDTO>.FailureResponse("Thương hiệu không tồn tại", "Lỗi khi tạo sản phẩm");
                    }
                }

                //B3: Generate slug từ tên và validate slug
                string slug = GenerateSlug(request.Name);
                if (!await _unitOfWork.Products.IsSlugUnique(slug))
                {
                    return ApiResponse<ProductDTO>.FailureResponse("Đường dẫn đã tồn tại, vui lòng chọn tên khác", "Lỗi khi tạo sản phẩm");
                }
                var product = new Product
                {
                    Name = request.Name,
                    Slug = slug,
                    Description = request.Description,
                    Content = request.Content,
                    Price = request.Price,
                    OriginalPrice = request.OriginalPrice,
                    Thumbnail = request.Thumbnail,
                    CategoryId = request.CategoryId,
                    BrandId = request.BrandId, // Nullable - OK
                    MetaTitle = request.MetaTitle ?? request.Name,
                    MetaKeyword = request.MetaKeyword,
                    MetaDescription = request.MetaDescription ?? request.Description,
                    IsFeatured = request.IsFeatured,
                    IsActive = request.IsActive
                };

                await _unitOfWork.Products.AddAsync(product);
                await _unitOfWork.SaveChangesAsync();

                // ⭐ BƯỚC 5: RELOAD entity để có relationships cho mapping
                var createdProduct = await _unitOfWork.Products.GetByIdAsync(product.Id);

                var productDTO = MapToDTO(createdProduct!);
                return ApiResponse<ProductDTO>.SuccessResponse(productDTO, "Tạo sản phẩm thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductDTO>.FailureResponse(ex.Message, "Lỗi khi tạo sản phẩm");
            }

        }

        public async Task<ApiResponse<ProductDTO>> UpdateProductAsync(Guid id, ProductUpdateRequest request)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product == null)
                {
                    return ApiResponse<ProductDTO>.FailureResponse("Sản phẩm không tồn tại", "Lỗi khi cập nhật sản phẩm");

                }
                var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
                if (category == null)
                {
                    return ApiResponse<ProductDTO>.FailureResponse("Danh mục không tồn tại", "Lỗi khi cập nhật sản phẩm");
                }
                if (request.BrandId.HasValue)
                {
                    var brand = await _unitOfWork.Brands.GetByIdAsync(request.BrandId.Value);
                    if (brand == null)
                    {
                        return ApiResponse<ProductDTO>.FailureResponse("Thương hiệu không tồn tại", "Lỗi khi cập nhật sản phẩm");
                    }
                }
                string newSlug = GenerateSlug(request.Name);
                if (newSlug != product.Slug)
                {
                    if (!await _unitOfWork.Products.IsSlugUnique(newSlug, id))
                    {
                        return ApiResponse<ProductDTO>.FailureResponse("Đường dẫn đã tồn tại, vui lòng chọn tên khác", "Lỗi khi cập nhật sản phẩm");
                    }
                    product.Slug = newSlug;
                }

                product.Name = request.Name;
                product.Description = request.Description;
                product.Content = request.Content;
                product.Price = request.Price;
                product.OriginalPrice = request.OriginalPrice;
                product.Thumbnail = request.Thumbnail;
                product.CategoryId = request.CategoryId;
                product.BrandId = request.BrandId;
                product.MetaTitle = request.MetaTitle ?? request.Name;
                product.MetaKeyword = request.MetaKeyword;
                product.MetaDescription = request.MetaDescription ?? request.Description;
                product.IsFeatured = request.IsFeatured;
                product.IsActive = request.IsActive;

                _unitOfWork.Products.Update(product);
                await _unitOfWork.SaveChangesAsync();

                // ⭐ BƯỚC 6: RELOAD entity
                var updatedProduct = await _unitOfWork.Products.GetByIdAsync(id);
                var productDTO = MapToDTO(updatedProduct!);

                return ApiResponse<ProductDTO>.SuccessResponse(productDTO, "Cập nhật sản phẩm thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductDTO>.FailureResponse(ex.Message, "Lỗi khi cập nhật sản phẩm");
            }

        }

        public async Task<ApiResponse<bool>> DeleteProductAsync(Guid id)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product == null)
                {
                    return ApiResponse<bool>.FailureResponse("Sản phẩm không tồn tại", "Lỗi khi xóa sản phẩm");

                }

                _unitOfWork.Products.SoftDelete(product);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResponse(true, "Xóa sản phẩm thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResponse(ex.Message, "Lỗi khi xóa sản phẩm");
            }
        }

        public async Task<ApiResponse<List<ProductDTO>>> GetAllProductsAsync()
        {
            try
            {
                var products = await _unitOfWork.Products.GetAllAsync();
                var productDTOs = products.Select(p => MapToDTO(p)).ToList();

                return ApiResponse<List<ProductDTO>>.SuccessResponse(productDTOs, "Lấy sản phẩm thành công");

            }
            catch (Exception ex)
            {
                return ApiResponse<List<ProductDTO>>.FailureResponse(ex.Message, "Lỗi khi lấy sản phẩm");
            }
        }

        public async Task<ApiResponse<ProductDTO>> GetProductByIdAsync(Guid id)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product == null)
                {
                    return ApiResponse<ProductDTO>.FailureResponse("Không tìm thấy sản phẩm", "");
                }
                var productDTO = MapToDTO(product);
                return ApiResponse<ProductDTO>.SuccessResponse(productDTO, "Product retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductDTO>.FailureResponse(ex.Message, "Error retrieving product");
            }
        }

        public async Task<ApiResponse<List<ProductDTO>>> GetFeaturedProductsAsync()
        {
            try
            {
                var products = await _unitOfWork.Products.GetFeaturedAsync();
                var productDtos = products.Select(MapToDTO).ToList();

                return ApiResponse<List<ProductDTO>>.SuccessResponse(productDtos, "Lấy sản phẩm nổi bật thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<ProductDTO>>.FailureResponse(ex.Message, "Lỗi khi lấy sản phẩm nổi bật");
            }
        }

        public async Task<ApiResponse<ProductDTO>> GetProductBySlugAsync(string slug)
        {
            try
            {
                var product = await _unitOfWork.Products.GetBySlugAsync(slug);

                if (product == null)
                {
                    return ApiResponse<ProductDTO>.FailureResponse("Product not found", "Error retrieving product");
                }

                var productDTO = MapToDTO(product);
                return ApiResponse<ProductDTO>.SuccessResponse(productDTO, "Product retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductDTO>.FailureResponse(ex.Message, "Error retrieving product");
            }
        }

        public async Task<ApiResponse<List<ProductDTO>>> GetProductsByBrandAsync(Guid brandId)
        {
            try
            {
                // Validate brand exists
                var brand = await _unitOfWork.Brands.GetByIdAsync(brandId);
                if (brand == null)
                {
                    return ApiResponse<List<ProductDTO>>.FailureResponse(
                        "Không tìm thấy thương hiệu"
                    );

                }

                var products = await _unitOfWork.Products.GetByBrandAsync(brandId);
                var productDtos = products.Select(MapToDTO).ToList();

                return ApiResponse<List<ProductDTO>>.SuccessResponse(productDtos, "Lấy sản phẩm theo thương hiệu thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<ProductDTO>>.FailureResponse(ex.Message, "Lỗi khi lấy sản phẩm theo thương hiệu");
            }
        }

        public async Task<ApiResponse<List<ProductDTO>>> GetProductsByCategoryAsync(Guid categoryId)
        {
            try
            {
                // Validate category exists
                var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
                if (category == null)
                {
                    return ApiResponse<List<ProductDTO>>.FailureResponse(
                        "Không tìm thấy danh mục"
                    );
                }

                var products = await _unitOfWork.Products.GetByCategoryAsync(categoryId);
                var productDtos = products.Select(MapToDTO).ToList();

                return ApiResponse<List<ProductDTO>>.SuccessResponse(productDtos, "Lấy sản phẩm theo danh mục thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<ProductDTO>>.FailureResponse(ex.Message, "Lỗi khi lấy sản phẩm theo danh mục");
            }
        }

        // HELPER METHODS
        private string GenerateSlug(string name)
        {
            string slug = name.ToLower();
            slug = Regex.Replace(slug, @"[àáạảãâầấậẩẫăằắặẳẵ]", "a");
            slug = Regex.Replace(slug, @"[èéẹẻẽêềếệểễ]", "e");
            slug = Regex.Replace(slug, @"[ìíịỉĩ]", "i");
            slug = Regex.Replace(slug, @"[òóọỏõôồốộổỗơờớợởỡ]", "o");
            slug = Regex.Replace(slug, @"[ùúụủũưừứựửữ]", "u");
            slug = Regex.Replace(slug, @"[ỳýỵỷỹ]", "y");
            slug = Regex.Replace(slug, @"[đ]", "d");
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", "-");
            slug = Regex.Replace(slug, @"-+", "-");
            return slug.Trim('-');
        }

        private ProductDTO MapToDTO(Product product)
        {
            return new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                Description = product.Description,
                Content = product.Content,
                Price = product.Price,
                OriginalPrice = product.OriginalPrice,
                Thumbnail = product.Thumbnail,
                IsActive = product.IsActive,
                IsFeatured = product.IsFeatured,
                ViewCount = product.ViewCount,
                SoldCount = product.SoldCount,
                MetaTitle = product.MetaTitle,
                MetaKeyword = product.MetaKeyword,
                MetaDescription = product.MetaDescription,

                // ⭐ Map nested Category (required)
                Category = new CategorySimpleDTO
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name,
                    Slug = product.Category.Slug
                },

                // ⭐ Map nested Brand (optional - nullable)
                Brand = product.Brand != null ? new BrandSimpleDTO
                {
                    Id = product.Brand.Id,
                    Name = product.Brand.Name,
                    Slug = product.Brand.Slug
                } : null,

                CreatedAt = product.CreatedAt
            };
        }
    }
}
