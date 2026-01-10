using System.Text.RegularExpressions;
using ClothingShop.Application.DTOs.Brand;
using ClothingShop.Application.Services.BrandService.Interfaces;
using ClothingShop.Application.Wrapper;
using ClothingShop.Domain.Entities;
using ClothingShop.Infrastructure.Interfaces;

namespace ClothingShop.Application.Services.BrandService.Impl
{
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;
        public BrandService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<BrandDTO>>> GetAllBrandsAsync()
        {
            try
            {
                var brands = await _unitOfWork.Brands.GetAllAsync();
                var brandDTOs = brands.Select(MapToDTO).OrderBy(b => b.Name).ToList();

                return ApiResponse<List<BrandDTO>>.SuccessResponse(brandDTOs, "Lấy danh sách thương hiệu thành công", System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<BrandDTO>>.FailureResponse("Đã xảy ra lỗi khi lấy danh sách thương hiệu", "ServerError", System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponse<BrandDTO>> GetBrandByIdAsync(Guid id)
        {
            try
            {
                var brand = await _unitOfWork.Brands.GetByIdAsync(id);

                if (brand == null)
                {
                    return ApiResponse<BrandDTO>.FailureResponse("Thương hiệu không tồn tại", "NotFound", System.Net.HttpStatusCode.NotFound);
                }

                var brandDTO = MapToDTO(brand);
                return ApiResponse<BrandDTO>.SuccessResponse(brandDTO, "Lấy thông tin thương hiệu thành công", System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return ApiResponse<BrandDTO>.FailureResponse("Đã xảy ra lỗi khi lấy thông tin thương hiệu", "ServerError", System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponse<BrandDTO>> CreateBrandAsync(BrandCreateRequest request)
        {
            try
            {
                var slug = string.IsNullOrWhiteSpace(request.Slug) ? GenerateSlug(request.Name) : GenerateSlug(request.Slug);

                var isSlugUnique = await _unitOfWork.Brands.IsSlugUnique(slug);
                if (!isSlugUnique)
                {
                    return ApiResponse<BrandDTO>.FailureResponse("Slug đã được sử dụng", "ValidationError", System.Net.HttpStatusCode.BadRequest);
                }

                var newBrand = new Brand
                {
                    Name = request.Name.Trim(),
                    Slug = slug,
                    LogoUrl = request.LogoUrl?.Trim(),
                    Website = request.Website?.Trim(),
                    Description = request.Description?.Trim()
                };

                await _unitOfWork.Brands.AddAsync(newBrand);
                await _unitOfWork.SaveChangesAsync();

                var brandDTO = MapToDTO(newBrand);
                return ApiResponse<BrandDTO>.SuccessResponse(brandDTO, "Tạo thương hiệu thành công", System.Net.HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return ApiResponse<BrandDTO>.FailureResponse("Đã xảy ra lỗi khi tạo thương hiệu", "ServerError", System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponse<BrandDTO>> UpdateBrandAsync(Guid id, BrandCreateRequest request)
        {
            try
            {
                var brand = await _unitOfWork.Brands.GetByIdAsync(id);
                if (brand == null)
                {
                    return ApiResponse<BrandDTO>.FailureResponse("Thương hiệu không tồn tại", "NotFound", System.Net.HttpStatusCode.NotFound);
                }

                var slug = string.IsNullOrWhiteSpace(request.Slug) ? GenerateSlug(request.Name) : GenerateSlug(request.Slug);

                if (slug != brand.Slug)
                {
                    var isSlugUnique = await _unitOfWork.Brands.IsSlugUnique(slug);
                    if (!isSlugUnique)
                    {
                        return ApiResponse<BrandDTO>.FailureResponse("Slug đã được sử dụng", "ValidationError", System.Net.HttpStatusCode.BadRequest);
                    }
                }

                brand.Name = request.Name.Trim();
                brand.Slug = slug;
                brand.LogoUrl = request.LogoUrl?.Trim();
                brand.Website = request.Website?.Trim();
                brand.Description = request.Description?.Trim();

                await _unitOfWork.Brands.UpdateAsync(brand);
                await _unitOfWork.SaveChangesAsync();
                var brandDTO = MapToDTO(brand);
                return ApiResponse<BrandDTO>.SuccessResponse(brandDTO, "Cập nhật thương hiệu thành công", System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return ApiResponse<BrandDTO>.FailureResponse("Đã xảy ra lỗi khi cập nhật thương hiệu", "ServerError", System.Net.HttpStatusCode.InternalServerError);
            }
        }


        public async Task<ApiResponse<bool>> DeleteBrandAsync(Guid id)
        {
            try
            {
                var brand = await _unitOfWork.Brands.GetByIdAsync(id);
                if (brand == null)
                {
                    return ApiResponse<bool>.FailureResponse("Thương hiệu không tồn tại", "NotFound", System.Net.HttpStatusCode.NotFound);
                }

                // 2. TODO: Check xem brand có product nào không
                // var hasProducts = await _unitOfWork.Products.AnyAsync(p => p.BrandId == id);
                // if (hasProducts)
                // {
                //     return ApiResponse<bool>.FailureResponse(
                //         "Không thể xóa thương hiệu đang có sản phẩm",
                //         HttpStatusCode.Conflict
                //     );
                // }

                _unitOfWork.Brands.Delete(brand);
                await _unitOfWork.SaveChangesAsync();
                return ApiResponse<bool>.SuccessResponse(true, "Xóa thương hiệu thành công", System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResponse("Đã xảy ra lỗi khi xóa thương hiệu", "ServerError", System.Net.HttpStatusCode.InternalServerError);
            }
        }


        //HELPER METHODS
        private BrandDTO MapToDTO(Brand brand)
        {
            return new BrandDTO
            {
                Id = brand.Id,
                Name = brand.Name,
                Slug = brand.Slug ?? string.Empty,
                LogoUrl = brand.LogoUrl,
                Website = brand.Website,
                Description = brand.Description,
                CreatedAt = brand.CreatedAt
            };
        }

        private string GenerateSlug(string input)
        {
            // 1. Lowercase
            string slug = input.ToLowerInvariant().Trim();

            // 2. Xử lý tiếng Việt (optional - nếu cần)
            slug = RemoveVietnameseTones(slug);

            // 3. Thay khoảng trắng bằng dấu gạch ngang
            slug = Regex.Replace(slug, @"\s+", "-");

            // 4. Xóa ký tự đặc biệt (chỉ giữ chữ, số, gạch ngang)
            slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");

            // 5. Xóa nhiều gạch ngang liên tiếp
            slug = Regex.Replace(slug, @"-+", "-");

            // 6. Xóa gạch ngang đầu/cuối
            slug = slug.Trim('-');

            return slug;
        }

        private string RemoveVietnameseTones(string text)
        {
            string[] vietnameseSigns = new string[]
         {
                "aAeEoOuUiIdDyY",
                "áàạảãâấầậẩẫăắằặẳẵ",
                "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
                "éèẹẻẽêếềệểễ",
                "ÉÈẸẺẼÊẾỀỆỂỄ",
                "óòọỏõôốồộổỗơớờợởỡ",
                "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
                "úùụủũưứừựửữ",
                "ÚÙỤỦŨƯỨỪỰỬỮ",
                "íìịỉĩ",
                "ÍÌỊỈĨ",
                "đ",
                "Đ",
                "ýỳỵỷỹ",
                "ÝỲỴỶỸ"
         };

            for (int i = 1; i < vietnameseSigns.Length; i++)
            {
                for (int j = 0; j < vietnameseSigns[i].Length; j++)
                {
                    text = text.Replace(vietnameseSigns[i][j], vietnameseSigns[0][i - 1]);
                }
            }

            return text;
        }
    }
}
