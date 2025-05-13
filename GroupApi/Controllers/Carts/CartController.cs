using GroupApi.DTOs.Carts;
using GroupApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GroupApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _cartService.GetAllAsync();
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return StatusCode((int)result.Error!.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _cartService.GetByIdAsync();
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return StatusCode((int)result.Error!.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddToCartDto model)
        {
            if (model == null)
                return BadRequest("Invalid cart data.");

            var result = await _cartService.AddAsync(model.BookId, model.Quantity);

            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetById), new { id = result.Data.CartId }, result.Data);
            return StatusCode((int)result.Error!.StatusCode, result);


        }

        [HttpPut("{cartItemId}")]
        public async Task<IActionResult> Update(Guid cartItemId, [FromBody] UpdateCartDto model)
        {
            if (model == null || cartItemId != model.CartItemId)
                return BadRequest("Invalid cart item data.");

            var result = await _cartService.UpdateAsync(cartItemId, model.Quantity);

            if (result.IsSuccess)
                return Ok(result.Data);
            return StatusCode((int)result.Error!.StatusCode, result);
        }

        [HttpDelete("{cartItemId}")]
        public async Task<IActionResult> Remove(Guid cartItemId)
        {
            var result = await _cartService.RemoveAsync(cartItemId);

            if (result.IsSuccess)
                return Ok(result.Data);
            return StatusCode((int)result.Error!.StatusCode, result);
        }
    }
}
