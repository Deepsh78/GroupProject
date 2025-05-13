using GroupApi.CommonDomain;
using GroupApi.DTOs;
using GroupApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.Extensions.Logging;
using GroupApi.DTOs.Files;
using GroupApi.Entities.Books;
using GroupApi.GenericClasses;
using Microsoft.EntityFrameworkCore;

namespace GroupApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IGenericRepository<Book> _bookRepo;
        private readonly ILogger<FileController> _logger;

        public FileController(
            IFileStorageService fileStorageService,
            IGenericRepository<Book> bookRepo,
            ILogger<FileController> logger)
        {
            _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
            _bookRepo = bookRepo ?? throw new ArgumentNullException(nameof(bookRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _logger.LogDebug("FileController initialized successfully.");
        }

        [HttpPost("upload/book/{bookId}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadBookImage(Guid bookId, [FromForm] FileUploadDto dto)
        {
            try
            {
                _logger.LogInformation("Processing upload request for bookId: {BookId}", bookId);

                if (dto == null || dto.File == null || dto.File.Length == 0)
                {
                    _logger.LogWarning("Invalid file upload request for bookId: {BookId}. File is null or empty.", bookId);
                    return BadRequest(new ErrorModel(HttpStatusCode.BadRequest, "No file uploaded or invalid request"));
                }

                var book = await _bookRepo.GetByIdAsync(bookId);
                if (book == null)
                {
                    _logger.LogWarning("Book not found for bookId: {BookId}", bookId);
                    return NotFound(new ErrorModel(HttpStatusCode.NotFound, "Book not found"));
                }

                var filePath = await _fileStorageService.SaveFileAsync(dto.File);
                book.BookImage = filePath;
                _bookRepo.Update(book);
                await _bookRepo.SaveChangesAsync();

                _logger.LogInformation("Image uploaded successfully for bookId: {BookId}, path: {FilePath}", bookId, filePath);
                return Ok(new { FilePath = filePath });
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "File system error uploading file for bookId: {BookId}", bookId);
                return StatusCode(500, new ErrorModel(HttpStatusCode.InternalServerError, $"File system error: {ex.Message}"));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error updating book for bookId: {BookId}", bookId);
                return StatusCode(500, new ErrorModel(HttpStatusCode.InternalServerError, $"Database error: {ex.Message}"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error uploading file for bookId: {BookId}", bookId);
                return StatusCode(500, new ErrorModel(HttpStatusCode.InternalServerError, $"Unexpected error: {ex.Message}"));
            }
        }

        [HttpGet("book/{bookId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBookImage(Guid bookId)
        {
            try
            {
                _logger.LogInformation("Retrieving image for bookId: {BookId}", bookId);

                var book = await _bookRepo.GetByIdAsync(bookId);
                if (book == null || string.IsNullOrEmpty(book.BookImage))
                {
                    _logger.LogWarning("Image not found for bookId: {BookId}", bookId);
                    return NotFound(new ErrorModel(HttpStatusCode.NotFound, "Image not found"));
                }

                var fileBytes = await _fileStorageService.GetFileAsync(book.BookImage);
                if (fileBytes == null)
                {
                    _logger.LogWarning("File not found on server for bookId: {BookId}, path: {FilePath}", bookId, book.BookImage);
                    return NotFound(new ErrorModel(HttpStatusCode.NotFound, "File not found on server"));
                }

                var extension = Path.GetExtension(book.BookImage).ToLower();
                string contentType = extension switch
                {
                    ".jpg" => "image/jpeg",
                    ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    _ => "application/octet-stream"
                };

                _logger.LogInformation("Image retrieved for bookId: {BookId}, path: {FilePath}", bookId, book.BookImage);
                return File(fileBytes, contentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving image for bookId: {BookId}", bookId);
                return StatusCode(500, new ErrorModel(HttpStatusCode.InternalServerError, $"Error retrieving file: {ex.Message}"));
            }
        }

        [HttpPut("book/{bookId}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateBookImage(Guid bookId, [FromForm] FileUploadDto dto)
        {
            try
            {
                _logger.LogInformation("Processing update request for bookId: {BookId}", bookId);

                if (dto == null || dto.File == null || dto.File.Length == 0)
                {
                    _logger.LogWarning("Invalid file update request for bookId: {BookId}. File is null or empty.", bookId);
                    return BadRequest(new ErrorModel(HttpStatusCode.BadRequest, "No file uploaded or invalid request"));
                }

                var book = await _bookRepo.GetByIdAsync(bookId);
                if (book == null)
                {
                    _logger.LogWarning("Book not found for bookId: {BookId}", bookId);
                    return NotFound(new ErrorModel(HttpStatusCode.NotFound, "Book not found"));
                }

                if (!string.IsNullOrEmpty(book.BookImage))
                {
                    await _fileStorageService.DeleteFileAsync(book.BookImage);
                    _logger.LogInformation("Deleted old image for bookId: {BookId}, path: {FilePath}", bookId, book.BookImage);
                }

                var filePath = await _fileStorageService.SaveFileAsync(dto.File);
                book.BookImage = filePath;
                _bookRepo.Update(book);
                await _bookRepo.SaveChangesAsync();

                _logger.LogInformation("Image updated successfully for bookId: {BookId}, path: {FilePath}", bookId, filePath);
                return Ok(new { FilePath = filePath });
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "File system error updating file for bookId: {BookId}", bookId);
                return StatusCode(500, new ErrorModel(HttpStatusCode.InternalServerError, $"File system error: {ex.Message}"));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error updating book for bookId: {BookId}", bookId);
                return StatusCode(500, new ErrorModel(HttpStatusCode.InternalServerError, $"Database error: {ex.Message}"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating file for bookId: {BookId}", bookId);
                return StatusCode(500, new ErrorModel(HttpStatusCode.InternalServerError, $"Unexpected error: {ex.Message}"));
            }
        }

        [HttpDelete("book/{bookId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBookImage(Guid bookId)
        {
            try
            {
                _logger.LogInformation("Processing delete request for bookId: {BookId}", bookId);

                var book = await _bookRepo.GetByIdAsync(bookId);
                if (book == null || string.IsNullOrEmpty(book.BookImage))
                {
                    _logger.LogWarning("Image not found for bookId: {BookId}", bookId);
                    return NotFound(new ErrorModel(HttpStatusCode.NotFound, "Image not found"));
                }

                await _fileStorageService.DeleteFileAsync(book.BookImage);
                book.BookImage = null;
                _bookRepo.Update(book);
                await _bookRepo.SaveChangesAsync();

                _logger.LogInformation("Image deleted successfully for bookId: {BookId}", bookId);
                return NoContent();
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "File system error deleting file for bookId: {BookId}", bookId);
                return StatusCode(500, new ErrorModel(HttpStatusCode.InternalServerError, $"File system error: {ex.Message}"));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error updating book for bookId: {BookId}", bookId);
                return StatusCode(500, new ErrorModel(HttpStatusCode.InternalServerError, $"Database error: {ex.Message}"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting file for bookId: {BookId}", bookId);
                return StatusCode(500, new ErrorModel(HttpStatusCode.InternalServerError, $"Unexpected error: {ex.Message}"));
            }
        }
    }
}