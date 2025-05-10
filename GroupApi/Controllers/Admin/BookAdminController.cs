using GroupApi.CommonDomain;
using GroupApi.DTOs.Books;
using GroupApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace GroupApi.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/books")]
    public class BookAdminController : ControllerBase
    {
        private readonly IBookAdminService _bookAdminService;

        public BookAdminController(IBookAdminService bookAdminService)
        {
            _bookAdminService = bookAdminService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorModel(HttpStatusCode.BadRequest, "Invalid input data"));

            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminId))
                return Unauthorized(new ErrorModel(HttpStatusCode.Unauthorized, "Admin not authenticated"));

            var result = await _bookAdminService.CreateAsync(dto, adminId);
            return result.IsSuccess
                ? CreatedAtAction(nameof(GetById), new { id = result.Data!.BookId }, result)
                : StatusCode((int)result.Error!.StatusCode, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _bookAdminService.GetAllAsync();
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.Error!.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _bookAdminService.GetByIdAsync(id);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.Error!.StatusCode, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] BookUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorModel(HttpStatusCode.BadRequest, "Invalid input data"));

            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminId))
                return Unauthorized(new ErrorModel(HttpStatusCode.Unauthorized, "Admin not authenticated"));

            var result = await _bookAdminService.UpdateAsync(id, dto, adminId);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.Error!.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminId))
                return Unauthorized(new ErrorModel(HttpStatusCode.Unauthorized, "Admin not authenticated"));

            var result = await _bookAdminService.DeleteAsync(id, adminId);
            return result.IsSuccess ? NoContent() : StatusCode((int)result.Error!.StatusCode, result);
        }

        [HttpPut("{id}/stock")]
        public async Task<IActionResult> UpdateStock(Guid id, [FromBody] UpdateStockDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorModel(HttpStatusCode.BadRequest, "Invalid input data"));

            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminId))
                return Unauthorized(new ErrorModel(HttpStatusCode.Unauthorized, "Admin not authenticated"));

            var result = await _bookAdminService.UpdateStockAsync(id, dto, adminId);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.Error!.StatusCode, result);
        }
    }
}