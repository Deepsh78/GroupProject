using GroupApi.CommonDomain;
using GroupApi.Constants;
using GroupApi.DTOs.Books;
using GroupApi.Entities;
using GroupApi.Entities.Auth;
using GroupApi.Entities.Books;
using GroupApi.GenericClasses;
using GroupApi.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GroupApi.Services.Books
{
    public class BookAdminService : IBookAdminService
    {
        private readonly IGenericRepository<Book> _bookRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGenericRepository<Publisher> _publisherRepo;

        public BookAdminService(
            IGenericRepository<Book> bookRepo,
            UserManager<ApplicationUser> userManager,
            IGenericRepository<Publisher> publisherRepo)
        {
            _bookRepo = bookRepo;
            _userManager = userManager;
            _publisherRepo = publisherRepo;
        }

        public async Task<GenericResponse<IEnumerable<BookReadDto>>> GetAllAsync()
        {
            var books = await _bookRepo.TableNoTracking
                .Include(b => b.Publisher)
                .ToListAsync();

            var result = books.Select(b => new BookReadDto
            {
                BookId = b.BookId,
                BookName = b.BookName,
                ISBN = b.ISBN,
                Price = b.Price,
                Description = b.Description,
                Language = b.Language,
                Stock = b.Stock,
                PublisherId = b.PublisherId,
                PublisherName = b.Publisher?.Name ?? "",
                PublicationDate = b.PublicationDate,
                CreatedAt = b.CreatedAt,
                IsComingSoon = b.IsComingSoon
            });

            return result.ToList();
        }

        public async Task<GenericResponse<BookReadDto?>> GetByIdAsync(Guid id)
        {
            var book = await _bookRepo.TableNoTracking
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null)
                return new ErrorModel(HttpStatusCode.NotFound, "Book not found");

            return new BookReadDto
            {
                BookId = book.BookId,
                BookName = book.BookName,
                ISBN = book.ISBN,
                Price = book.Price,
                Description = book.Description,
                Language = book.Language,
                Stock = book.Stock,
                PublisherId = book.PublisherId,
                PublisherName = book.Publisher?.Name ?? "",
                PublicationDate = book.PublicationDate,
                CreatedAt = book.CreatedAt,
                IsComingSoon = book.IsComingSoon
            };
        }

        public async Task<GenericResponse<BookReadDto>> CreateAsync(BookCreateDto dto, string adminId)
        {
            var admin = await _userManager.FindByIdAsync(adminId);
            if (admin == null || admin.Role != RoleType.Admin)
                return new ErrorModel(HttpStatusCode.Forbidden, "Only admins can create books");

            var publisher = await _publisherRepo.GetByIdAsync(dto.PublisherId);
            if (publisher == null)
                return new ErrorModel(HttpStatusCode.BadRequest, "Invalid publisher ID");

            var book = new Book
            {
                BookId = Guid.NewGuid(),
                BookName = dto.BookName,
                ISBN = dto.ISBN,
                Price = dto.Price,
                Description = dto.Description,
                Language = dto.Language,
                Stock = dto.Stock,
                PublisherId = dto.PublisherId,
                PublicationDate = dto.PublicationDate,
                IsComingSoon = dto.IsComingSoon,
                CreatedAt = DateTime.UtcNow
            };

            await _bookRepo.AddAsync(book);
            await _bookRepo.SaveChangesAsync();

            return new BookReadDto
            {
                BookId = book.BookId,
                BookName = book.BookName,
                ISBN = book.ISBN,
                Price = book.Price,
                Description = book.Description,
                Language = book.Language,
                Stock = book.Stock,
                PublisherId = book.PublisherId,
                PublisherName = publisher.Name,
                PublicationDate = book.PublicationDate,
                CreatedAt = book.CreatedAt,
                IsComingSoon = book.IsComingSoon
            };
        }

        public async Task<GenericResponse<BookReadDto?>> UpdateAsync(Guid id, BookUpdateDto dto, string adminId)
        {
            var admin = await _userManager.FindByIdAsync(adminId);
            if (admin == null || admin.Role != RoleType.Admin)
                return new ErrorModel(HttpStatusCode.Forbidden, "Only admins can update books");

            var book = await _bookRepo.GetByIdAsync(id);
            if (book == null)
                return new ErrorModel(HttpStatusCode.NotFound, "Book not found");

            var publisher = await _publisherRepo.GetByIdAsync(dto.PublisherId);
            if (publisher == null)
                return new ErrorModel(HttpStatusCode.BadRequest, "Invalid publisher ID");

            book.BookName = dto.BookName;
            book.ISBN = dto.ISBN;
            book.Price = dto.Price;
            book.Description = dto.Description;
            book.Language = dto.Language;
            book.Stock = dto.Stock;
            book.PublisherId = dto.PublisherId;
            book.PublicationDate = dto.PublicationDate;
            book.IsComingSoon = dto.IsComingSoon;

            _bookRepo.Update(book);
            await _bookRepo.SaveChangesAsync();

            return new BookReadDto
            {
                BookId = book.BookId,
                BookName = book.BookName,
                ISBN = book.ISBN,
                Price = book.Price,
                Description = book.Description,
                Language = book.Language,
                Stock = book.Stock,
                PublisherId = book.PublisherId,
                PublisherName = publisher.Name,
                PublicationDate = book.PublicationDate,
                CreatedAt = book.CreatedAt,
                IsComingSoon = book.IsComingSoon
            };
        }

        public async Task<Response> DeleteAsync(Guid id, string adminId)
        {
            var admin = await _userManager.FindByIdAsync(adminId);
            if (admin == null || admin.Role != RoleType.Admin)
                return new ErrorModel(HttpStatusCode.Forbidden, "Only admins can delete books");

            var book = await _bookRepo.GetByIdAsync(id);
            if (book == null)
                return new ErrorModel(HttpStatusCode.NotFound, "Book not found");

            _bookRepo.Delete(book);
            await _bookRepo.SaveChangesAsync();
            return new Response();
        }

        public async Task<GenericResponse<BookReadDto>> UpdateStockAsync(Guid id, UpdateStockDto dto, string adminId)
        {
            var admin = await _userManager.FindByIdAsync(adminId);
            if (admin == null || admin.Role != RoleType.Admin)
                return new ErrorModel(HttpStatusCode.Forbidden, "Only admins can update stock");

            var book = await _bookRepo.GetByIdAsync(id);
            if (book == null)
                return new ErrorModel(HttpStatusCode.NotFound, "Book not found");

            if (dto.Stock < 0)
                return new ErrorModel(HttpStatusCode.BadRequest, "Stock cannot be negative");

            book.Stock = dto.Stock;
            _bookRepo.Update(book);
            await _bookRepo.SaveChangesAsync();

            var publisher = await _publisherRepo.GetByIdAsync(book.PublisherId);

            return new BookReadDto
            {
                BookId = book.BookId,
                BookName = book.BookName,
                ISBN = book.ISBN,
                Price = book.Price,
                Description = book.Description,
                Language = book.Language,
                Stock = book.Stock,
                PublisherId = book.PublisherId,
                PublisherName = publisher?.Name ?? "",
                PublicationDate = book.PublicationDate,
                CreatedAt = book.CreatedAt,
                IsComingSoon = book.IsComingSoon
            };
        }
    }
}