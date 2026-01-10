using System.Net;
using System.Text.RegularExpressions;
using ClothingShop.Application.DTOs.Category;
using ClothingShop.Application.Services.CategoryService.Interfaces;
using ClothingShop.Application.Wrapper;
using ClothingShop.Domain.Entities;
using ClothingShop.Infrastructure.Interfaces;

namespace ClothingShop.Application.Services.CategoryService.Impl
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<CategoryDTO>>> GetAllCategoriesRecursive()
        {
            try
            {
                var allData = await _unitOfWork.Categories.GetAllAsync();
                var activeData = allData.Where(x => x.IsActive).OrderBy(x => x.Name).ToList();
                var rootNodes = activeData.Where(x => x.ParentId == null).ToList();
                var result = new List<CategoryDTO>();

                foreach (var root in rootNodes)
                {
                    var dto = MapToDto(root);
                    dto.Children = GetChildrenRecursive(root.Id, activeData);
                    result.Add(dto);
                }

                return ApiResponse<List<CategoryDTO>>.SuccessResponse(result, "Lấy danh sách danh mục thành công", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CategoryDTO>>.FailureResponse("Đã xảy ra lỗi khi lấy danh sách danh mục", "ServerError", HttpStatusCode.InternalServerError);
            }
        }

        // Đã sửa tên hàm bỏ bớt 1 chữ Async cho khớp Interface sửa ở trên
        public async Task<ApiResponse<CategoryDTO>> GetCategoryByIdAsync(Guid id)
        {
            try
            {
                var allData = await _unitOfWork.Categories.GetAllAsync();
                var category = allData.FirstOrDefault(c => c.Id == id);

                if (category == null)
                {
                    return ApiResponse<CategoryDTO>.FailureResponse("Danh mục không tồn tại", "NotFound", HttpStatusCode.NotFound);
                }

                var categoryDto = MapToDto(category);
                return ApiResponse<CategoryDTO>.SuccessResponse(categoryDto, "Lấy thông tin danh mục thành công", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return ApiResponse<CategoryDTO>.FailureResponse("Đã xảy ra lỗi khi lấy thông tin danh mục", "ServerError", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponse<bool>> CreateCategoryAsync(CategoryCreateRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return ApiResponse<bool>.FailureResponse("Tên danh mục không được để trống", "ValidationError", HttpStatusCode.BadRequest);
                }

                var newCategory = new Category
                {
                    Name = request.Name,
                    ParentId = request.ParentId,
                    Description = request.Description,
                    IconUrl = request.IconUrl,
                    IsActive = true,
                    Slug = GenerateSlug(request.Name)
                };

                await _unitOfWork.Categories.AddAsync(newCategory);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResponse(true, "Tạo danh mục mới thành công", HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResponse("Đã xảy ra lỗi khi tạo danh mục", "ServerError", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponse<bool>> UpdateCategoryAsync(Guid id, CategoryCreateRequest request)
        {
            try
            {
                var allData = await _unitOfWork.Categories.GetAllAsync();
                var category = allData.FirstOrDefault(c => c.Id == id);

                if (category == null)
                {
                    return ApiResponse<bool>.FailureResponse("Danh mục không tồn tại", "NotFound", HttpStatusCode.NotFound);
                }

                if (request.ParentId == id)
                {
                    return ApiResponse<bool>.FailureResponse("Danh mục cha không thể là chính nó", "ValidationError", HttpStatusCode.BadRequest);
                }

                category.Name = request.Name;
                category.ParentId = request.ParentId;
                category.Description = request.Description;
                category.IconUrl = request.IconUrl;
                category.Slug = GenerateSlug(request.Name);

                await _unitOfWork.Categories.UpdateAsync(category);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResponse(true, "Cập nhật danh mục thành công", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResponse("Đã xảy ra lỗi khi cập nhật danh mục", "ServerError", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponse<bool>> DeleteCategoryAsync(Guid id)
        {
            try
            {
                var allData = await _unitOfWork.Categories.GetAllAsync();
                var category = allData.FirstOrDefault(c => c.Id == id);

                if (category == null)
                {
                    return ApiResponse<bool>.FailureResponse("Danh mục không tồn tại", "NotFound", HttpStatusCode.NotFound);
                }

                bool hasChildren = allData.Any(c => c.ParentId == id);
                if (hasChildren)
                {
                    return ApiResponse<bool>.FailureResponse("Không thể xóa danh mục đang chứa danh mục con", "Conflict", HttpStatusCode.Conflict);
                }

                _unitOfWork.Categories.Delete(category);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResponse(true, "Xóa danh mục thành công", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResponse("Đã xảy ra lỗi khi xóa danh mục", "ServerError", HttpStatusCode.InternalServerError);
            }
        }

        // --- PRIVATE HELPERS ---

        private List<CategoryDTO> GetChildrenRecursive(Guid parentId, List<Category> allData)
        {
            return allData.Where(c => c.ParentId == parentId)
                          .OrderBy(c => c.Name)
                          .Select(c =>
                          {
                              var dto = MapToDto(c);
                              dto.Children = GetChildrenRecursive(c.Id, allData);
                              return dto;
                          })
                          .ToList();
        }

        private CategoryDTO MapToDto(Category entity)
        {
            return new CategoryDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Slug = entity.Slug,
                IconUrl = entity.IconUrl,
                ParentId = entity.ParentId,
                Children = new List<CategoryDTO>()
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