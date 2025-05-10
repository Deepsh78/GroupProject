using GroupApi.CommonDomain;
using GroupApi.DTOs.Orders;
using GroupApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GroupApi.Controllers.Orders
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("{orderId}/claim-code")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<GenericResponse<ClaimCodeDto>>> GenerateClaimCode(Guid orderId)
        {
            var result = await _orderService.GenerateClaimCodeAsync(orderId);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.Error!.StatusCode, result);
        }

        [HttpPost("process-claim-code")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<GenericResponse<ClaimCodeDto>>> ProcessClaimCode([FromBody] ProcessClaimCodeDto dto)
        {
            var staffId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _orderService.ProcessClaimCodeAsync(dto, staffId);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.Error!.StatusCode, result);
        }
    }
}