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

        // Đã sửa tên hàm bỏ bớt 1 chữ Async cho khớp Interface sửa ở trên
        public async Task<ApiResponse<CategoryDTO>> GetCategoryByIdAsync(Guid id)
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

        public async Task<ApiResponse<bool>> CreateCategoryAsync(CategoryCreateRequest request)
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

        public async Task<ApiResponse<bool>> UpdateCategoryAsync(Guid id, CategoryCreateRequest request)
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

        public async Task<ApiResponse<bool>> DeleteCategoryAsync(Guid id)
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

        private string GenerateSlug(string name)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            string str = name.ToLower();
            str = Regex.Replace(str, @"[áàạảãâấầậẩẫăắằặẳẵ]", "a");
            str = Regex.Replace(str, @"[éèẹẻẽêếềệểễ]", "e");
            str = Regex.Replace(str, @"[óòọỏõôốồộổỗơớờợởỡ]", "o");
            str = Regex.Replace(str, @"[úùụủũưứừựửữ]", "u");
            str = Regex.Replace(str, @"[íìịỉĩ]", "i");
            str = Regex.Replace(str, @"[đ]", "d");
            str = Regex.Replace(str, @"[ýỳỵỷỹ]", "y");
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", "-").Trim();
            return str;
        }
    }
}