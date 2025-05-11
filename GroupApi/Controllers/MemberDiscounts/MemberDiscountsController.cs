using DocumentFormat.OpenXml.Spreadsheet;
using GroupApi.DTOs.Carts;
using GroupApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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
        [HttpPost("apply-discount")]
        public async Task<IActionResult> ApplyDiscount([FromBody] List<CartItemDto> cartItems)
        {
            var result = await _memberDiscountService.GetDiscountAsync( cartItems);
            if (result.IsSuccess)
                return Ok(result.Data);
            return StatusCode((int)result.Error!.StatusCode, result);
        }
    }
    
       
}
