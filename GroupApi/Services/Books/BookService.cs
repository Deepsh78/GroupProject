using GroupApi.CommonDomain;
using GroupApi.DTOs.Books;
using GroupApi.Entities;
using GroupApi.Entities.Books;
using GroupApi.GenericClasses;
using GroupApi.Pagination;
using GroupApi.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GroupApi.Services.Books
{
    public class BookService : IBookService
    {
        private readonly IGenericRepository<Book> _bookRepo;
        private readonly IGenericRepository<Publisher> _publisherRepo;

        public BookService(IGenericRepository<Book> bookRepo, IGenericRepository<Publisher> publisherRepo)
        {
            _bookRepo = bookRepo;
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

        public async Task<GenericResponse<BookReadDto>> CreateAsync(BookCreateDto dto)
        {
            try
            {
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
                    PublisherName = "", // You may fetch it if needed
                    PublicationDate = book.PublicationDate,
                    CreatedAt = book.CreatedAt,
                    IsComingSoon = book.IsComingSoon
                };
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here if needed
                // You can use a logging framework like Serilog, NLog, etc.
                // For now, just return an error response
                {
                    return new ErrorModel(HttpStatusCode.BadRequest, "Error creating book");
                }
            }
        }
        public async Task<GenericResponse<BookReadDto?>> UpdateAsync(Guid id, BookUpdateDto dto)
        {
            var book = await _bookRepo.GetByIdAsync(id);
            if (book == null)
                return new ErrorModel(HttpStatusCode.NotFound, "Book not found");

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
                PublisherName = "", // optional
                PublicationDate = book.PublicationDate,
                CreatedAt = book.CreatedAt,
                IsComingSoon = book.IsComingSoon
            };
        }

        public async Task<Response> DeleteAsync(Guid id)
        {
            var book = await _bookRepo.GetByIdAsync(id);
            if (book == null)
                return new ErrorModel(HttpStatusCode.NotFound, "Book not found");

            _bookRepo.Delete(book);
            await _bookRepo.SaveChangesAsync();
            return new Response();
        }

        public async Task<PaginatedList<BookReadDto>> GetFilteredBooksAsync(BookFilterDto filter)
        {
            var query = _bookRepo.TableNoTracking
                .Include(b => b.Publisher)
          
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                query = query.Where(b =>
                    b.BookName.Contains(filter.SearchTerm) ||
                    b.Description.Contains(filter.SearchTerm) ||
                    b.ISBN.Contains(filter.SearchTerm));
            }

         
            query = filter.SortByPriceDescending
                ? query.OrderByDescending(b => b.Price)
                : query.OrderBy(b => b.Price);

            var projection = query.Select(b => new BookReadDto
            {
                BookId = b.BookId,
                BookName = b.BookName,
                ISBN = b.ISBN,
                Price = b.Price,
                Description = b.Description,
                Language = b.Language,
                PublisherName = b.Publisher.Name
            });

            return await projection.ToPagedListAsync(filter.Page, filter.PageSize);
        }
    }
   
    }
