using ClothingShop.Application.DTOs.Address;
using ClothingShop.Application.Services.AddressService.Interfaces;
using ClothingShop.Application.Services.Auth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClothingShop.API.Controllers
{
    [Route("api/address")]
    [ApiController]
    [Authorize]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;
        private readonly ICurrentUserService _currentUserService;
        public AddressController(IAddressService addressService, ICurrentUserService currentUserService)
        {
            _addressService = addressService;
            _currentUserService = currentUserService;
        }

        private Guid CurrentUserId => _currentUserService.UserId
                                      ?? throw new UnauthorizedAccessException("User ID is missing.");

        // GET: api/addresses
        [HttpGet]
        public async Task<IActionResult> GetMyAddressesAsync()
        {
            var response = await _addressService.GetUserAddressesAsync(CurrentUserId);
            return StatusCode(response.Status, response);
        }

        // GET: api/addresses/{id}
        [HttpGet]
        [Route("{addressId:guid}")]
        public async Task<IActionResult> GetAddressByIdAsync([FromRoute] Guid addressId)
        {
            var response = await _addressService.GetAddressByIdAsync(CurrentUserId, addressId);
            return StatusCode(response.Status, response);
        }

        // POST: api/address
        [HttpPost]
        public async Task<IActionResult> CreateAddressAsync([FromBody] CreateAddressRequest request)
        {
            var response = await _addressService.CreateAddressAsync(CurrentUserId, request);
            return StatusCode(response.Status, response);
        }

        // PUT: api/address/{id}
        [HttpPut]
        [Route("{addressId:guid}")]
        public async Task<IActionResult> UpdateAddressAsync([FromRoute] Guid addressId, [FromBody] UpdateAddressRequest request)
        {
            var response = await _addressService.UpdateAddressAsync(CurrentUserId, addressId, request);
            return StatusCode(response.Status, response);
        }

        // DELETE: api/address/{id}
        [HttpDelete]
        [Route("{addressId:guid}")]
        public async Task<IActionResult> DeleteAddressAsync([FromRoute] Guid addressId)
        {
            var response = await _addressService.DeleteAddressAsync(CurrentUserId, addressId);
            return StatusCode(response.Status, response);
        }

        //PATCH: api/addresses/{id}/default
        [HttpPatch]
        [Route("{addressId:guid}/default")]
        public async Task<IActionResult> SetDefaultAddressAsync([FromRoute] Guid addressId)
        {
            var response = await _addressService.SetDefaultAddressAsync(CurrentUserId, addressId);
            return StatusCode(response.Status, response);
        }

    }
}
