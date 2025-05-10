using DocumentFormat.OpenXml.Spreadsheet;
using GroupApi.CommonDomain;
using GroupApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GroupApi.Controllers.BookMarks
{
    [Route("api/bookmarks")]
    [ApiController]
    public class BookMarkController : ControllerBase
    {
        private readonly IBookMarkService _bookMarkService;

        public BookMarkController(IBookMarkService bookMarkService)
        {
            _bookMarkService = bookMarkService;
        }

        // Get all bookmarks for the current user
        [HttpGet]
        public async Task<IActionResult> GetBookmarks()
        {
            var memberId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;  // Get current user ID from claims
            var result = await _bookMarkService.GetBookmarksByMemberAsync(Guid.Parse(memberId));

            return result.IsSuccess ? Ok(result) : StatusCode((int)result.Error!.StatusCode, result);

           
        }

        // Add a bookmark for a member
        [HttpPost("{bookId}")]
        public async Task<IActionResult> AddBookmark(Guid bookId)
        {
            var result = await _bookMarkService.AddBookmarkAsync(bookId);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.Error!.StatusCode, result);
        }

        // Remove a bookmark for a member
        [HttpDelete("{bookId}")]
        public async Task<IActionResult> RemoveBookmark(Guid bookId)
        {
            var result = await _bookMarkService.RemoveBookmarkAsync(bookId);

            return result.IsSuccess ? Ok(result) : StatusCode((int)result.Error!.StatusCode, result);
        }
    }

}
