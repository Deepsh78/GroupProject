using GroupApi.DTOs.Carts;
using GroupApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace GroupApi.Controllers.MemberOrders
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberOrderController : ControllerBase
    {
        private readonly IMemberOrderService _memberOrderService;
        public MemberOrderController(IMemberOrderService memberOrderService)
        {
            _memberOrderService = memberOrderService;
        }
        [HttpPost("place-order")]
        public async Task<IActionResult> PlaceOrder([FromBody] List<CartItemDto> cartItems)
        {
            var result = await _memberOrderService.PlaceOrderAsync(cartItems);
            if (result.IsSuccess)
                return Ok(result.Data);
            return StatusCode((int)result.Error!.StatusCode, result);
        }
        [HttpPost("cancel-order")]
        public async Task<IActionResult> CancelOrder([FromBody] Guid orderId)
        {
            var result = await _memberOrderService.CancelOrderAsync(orderId);
            if (result.IsSuccess)
                return Ok(result.Data);
            return StatusCode((int)result.Error!.StatusCode, result);
        }
    }
}
