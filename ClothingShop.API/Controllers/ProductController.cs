using ClothingShop.Application.DTOs.Product;
using ClothingShop.Application.Services.ProductService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClothingShop.API.Controllers
{

    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // ============================================
        // GET: api/products
        // Lấy tất cả sản phẩm (Public)
        // ============================================
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var response = await _productService.GetAllProductsAsync();
            return StatusCode((int)response.Status, response);
        }

        // ============================================
        // GET: api/products/{id}
        // Lấy sản phẩm theo ID (Public)
        // ============================================
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _productService.GetProductByIdAsync(id);
            return StatusCode((int)response.Status, response);
        }

        // ============================================
        // GET: api/products/slug/{slug}
        // Lấy sản phẩm theo Slug (Public)
        // ============================================
        [HttpGet("slug/{slug}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var response = await _productService.GetProductBySlugAsync(slug);
            return StatusCode((int)response.Status, response);
        }

        // ============================================
        // GET: api/products/featured
        // Lấy sản phẩm nổi bật (Public)
        // ============================================
        [HttpGet("featured")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFeatured()
        {
            var response = await _productService.GetFeaturedProductsAsync();
            return StatusCode((int)response.Status, response);
        }

        // ============================================
        // GET: api/products/category/{categoryId}
        // Lấy sản phẩm theo Category (Public)
        // ============================================
        [HttpGet("category/{categoryId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByCategory(Guid categoryId)
        {
            var response = await _productService.GetProductsByCategoryAsync(categoryId);
            return StatusCode((int)response.Status, response);
        }

        // ============================================
        // GET: api/products/brand/{brandId}
        // Lấy sản phẩm theo Brand (Public)
        // ============================================
        [HttpGet("brand/{brandId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByBrand(Guid brandId)
        {
            var response = await _productService.GetProductsByBrandAsync(brandId);
            return StatusCode((int)response.Status, response);
        }

        // ============================================
        // POST: api/products
        // Tạo sản phẩm mới (Admin only)
        // ============================================
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] ProductCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _productService.CreateProductAsync(request);
            return StatusCode((int)response.Status, response);
        }

        // ============================================
        // PUT: api/products/{id}
        // Cập nhật sản phẩm (Admin only)
        // ============================================
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProductUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _productService.UpdateProductAsync(id, request);
            return StatusCode((int)response.Status, response);
        }

        // ============================================
        // DELETE: api/products/{id}
        // Xóa sản phẩm (Soft delete, Admin only)
        // ============================================
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _productService.DeleteProductAsync(id);
            return StatusCode((int)response.Status, response);
        }
    }
}

