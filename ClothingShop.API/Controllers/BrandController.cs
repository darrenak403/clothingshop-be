using ClothingShop.Application.DTOs.Brand;
using ClothingShop.Application.Services.BrandService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClothingShop.API.Controllers
{
    [Route("api/brands")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;
        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        // ============================================
        // GET: api/brands
        // Lấy tất cả thương hiệu (Public)
        // ============================================
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllBrandsAsync()
        {
            var response = await _brandService.GetAllBrandsAsync();
            return StatusCode((int)response.Status, response);
        }

        // ============================================
        // GET: api/brands/id
        // Lấy thương hiệu theo ID (Public)
        // ============================================
        [HttpGet("id")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBrandById(Guid id)
        {
            var response = await _brandService.GetBrandByIdAsync(id);
            return StatusCode((int)response.Status, response);
        }

        // ============================================
        // POST: api/brands
        // Tạo thương hiệu mới (Admin only)
        // ============================================
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBrand([FromBody] BrandCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _brandService.CreateBrandAsync(request);
            return StatusCode((int)response.Status, response);
        }

        // ============================================
        // PUT: api/brands/{id}
        // Cập nhật thương hiệu (Admin only)
        // ============================================
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBrand(Guid id, [FromBody] BrandCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _brandService.UpdateBrandAsync(id, request);
            return StatusCode((int)response.Status, response);
        }

        // ============================================
        // DELETE: api/brands/{id}
        // Xóa thương hiệu (Admin only)
        // ============================================
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBrand(Guid id)
        {
            var response = await _brandService.DeleteBrandAsync(id);
            return StatusCode((int)response.Status, response);
        }

    }
}
