using DocumentFormat.OpenXml.Spreadsheet;
using GroupApi.DTOs.Carts;
using GroupApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroupApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class MemberDiscountController : ControllerBase
    {
        private readonly IMemberDiscountService _memberDiscountService;

        public MemberDiscountController(IMemberDiscountService memberDiscountService)
        {
            _memberDiscountService = memberDiscountService;
        }

        [HttpPost("apply")]
        public async Task<IActionResult> ApplyDiscount([FromBody] List<CartItemDto> cartItems)
        {
            var memberId = User.Identity.Name;

            var result = await _memberDiscountService.PlaceOrderAsync(cartItems);
            if (result.IsSuccess)
                return Ok(result.Data);
            return StatusCode((int)result.Error!.StatusCode, result);
        }

        [HttpGet("next-order-discount")]
        public async Task<IActionResult> GetNextOrderDiscount()
        {
            var memberId = User.Identity.Name;

            var result = await _memberDiscountService.GetDiscountAsync(Guid.Parse(memberId), new List<CartItemDto>());
            if (result.IsSuccess)
                return Ok(result.Data);
            return StatusCode((int)result.Error!.StatusCode, result);
        }
    }
}
