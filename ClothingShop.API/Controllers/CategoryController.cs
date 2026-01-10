using ClothingShop.Application.DTOs.Category;
using ClothingShop.Application.Services.CategoryService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClothingShop.API.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/categories
        // Lấy danh sách cây thư mục (Public - Ai cũng xem được)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCategoriesRecursiveAsync()
        {
            var response = await _categoryService.GetAllCategoriesRecursive();
            // Lưu ý: Nếu ApiResponse dùng Enum StatusCode thì cast: (int)response.StatusCode
            // Ở đây tôi giữ .Status theo mẫu AddressController của bạn
            return StatusCode((int)response.Status, response);
        }

        // GET: api/categories/{id}
        // Lấy chi tiết (Public)
        [HttpGet]
        [Route("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryByIdAsync([FromRoute] Guid id)
        {
            var response = await _categoryService.GetCategoryByIdAsync(id);
            return StatusCode((int)response.Status, response);
        }

        // POST: api/categories
        // Tạo mới (Chỉ Admin)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategoryAsync([FromBody] CategoryCreateRequest request)
        {
            var response = await _categoryService.CreateCategoryAsync(request);
            return StatusCode((int)response.Status, response);
        }

        // PUT: api/categories/{id}
        // Cập nhật (Chỉ Admin)
        [HttpPut]
        [Route("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategoryAsync([FromRoute] Guid id, [FromBody] CategoryCreateRequest request)
        {
            var response = await _categoryService.UpdateCategoryAsync(id, request);
            return StatusCode((int)response.Status, response);
        }

        // DELETE: api/categories/{id}
        // Xóa (Chỉ Admin)
        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategoryAsync([FromRoute] Guid id)
        {
            var response = await _categoryService.DeleteCategoryAsync(id);
            return StatusCode((int)response.Status, response);
        }
    }
}