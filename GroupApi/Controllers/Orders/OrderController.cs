using GroupApi.CommonDomain;
using GroupApi.DTOs.Orders;
using GroupApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GroupApi.Controllers.Orders
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Staff")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("process-claim")]
        public async Task<ActionResult<GenericResponse<bool>>> ProcessClaimCode([FromBody] ProcessClaimCodeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorModel(System.Net.HttpStatusCode.BadRequest, "Invalid input data"));

            var staffId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(staffId))
                return Unauthorized(new ErrorModel(System.Net.HttpStatusCode.Unauthorized, "Staff not authenticated"));

            var result = await _orderService.ProcessClaimCodeAsync(dto, staffId);
            if (!result.IsSuccess)
                return StatusCode((int)result.Error!.StatusCode, result);

            return Ok(result);
        }
    }
}