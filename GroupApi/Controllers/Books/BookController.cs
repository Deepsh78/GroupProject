using GroupApi.DTOs.Books;
using GroupApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace GroupApi.Controllers.Books
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _bookService.GetAllAsync();
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.Error!.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _bookService.GetByIdAsync(id);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.Error!.StatusCode, result);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] BookCreateDto dto)
        {
            var result = await _bookService.CreateAsync(dto);
            return result.IsSuccess
                ? CreatedAtAction(nameof(GetById), new { id = result.Data!.BookId }, result)
                : BadRequest(result);
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(Guid id, [FromForm] BookUpdateDto dto)
        {
            var result = await _bookService.UpdateAsync(id, dto);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.Error!.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _bookService.DeleteAsync(id);
            return result.IsSuccess ? NoContent() : StatusCode((int)result.Error!.StatusCode, result);
        }

        [HttpGet("filtered-books")]
        public async Task<IActionResult> GetFilteredBooks([FromQuery] BookFilterDto filter)
        {
            var result = await _bookService.GetFilteredBooksAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetBookDetail(Guid id)
        {
            var result = await _bookService.GetBookDetailAsync(id);
            return result == null
                ? NotFound(new { message = "Book not found" })
                : Ok(result);
        }
    }
}