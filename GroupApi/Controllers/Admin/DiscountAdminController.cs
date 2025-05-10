using GroupApi.CommonDomain;
using GroupApi.DTOs.Discount;
using GroupApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace GroupApi.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/discounts")]
    public class DiscountAdminController : ControllerBase
    {
        private readonly IDiscountService _discountService;

        public DiscountAdminController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _discountService.GetAllAsync();
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.Error!.StatusCode, result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveDiscounts()
        {
            var result = await _discountService.GetActiveDiscountsAsync();
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.Error!.StatusCode, result);
        }

        [HttpGet("on-sale")]
        public async Task<IActionResult> GetOnSaleDiscounts()
        {
            var result = await _discountService.GetOnSaleDiscountsAsync();
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.Error!.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _discountService.GetByIdAsync(id);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.Error!.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DiscountCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorModel(HttpStatusCode.BadRequest, "Invalid input data"));

            var result = await _discountService.CreateAsync(dto);
            return result.IsSuccess
                ? CreatedAtAction(nameof(GetById), new { id = result.Data!.DiscountId }, result)
                : StatusCode((int)result.Error!.StatusCode, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] DiscountUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorModel(HttpStatusCode.BadRequest, "Invalid input data"));

            var result = await _discountService.UpdateAsync(id, dto);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.Error!.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _discountService.DeleteAsync(id);
            return result.IsSuccess ? NoContent() : StatusCode((int)result.Error!.StatusCode, result);
        }
    }
}