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

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllBrandsAsync()
        {
            var response = await _brandService.GetAllBrandsAsync();
            return StatusCode((int)response.Status, response);
        }

        [HttpGet("id")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBrandById(Guid id)
        {
            var response = await _brandService.GetBrandByIdAsync(id);
            return StatusCode((int)response.Status, response);
        }

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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBrand(Guid id)
        {
            var response = await _brandService.DeleteBrandAsync(id);
            return StatusCode((int)response.Status, response);
        }

    }
}
