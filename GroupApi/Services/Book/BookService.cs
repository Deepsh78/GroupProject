using GroupApi.Data;
using GroupApi.Dto.Book;
using GroupApi.Entities;
using GroupApi.Services.Interface.Book;
using Microsoft.EntityFrameworkCore;


namespace GroupApi.Services.Book
{
    public class BookService : IBookService
    {
        private readonly ApplicaionDbContext _context;

        public BookService(ApplicaionDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookReadDto>> GetAllBooksAsync()
        {
            return await _context.Books
                .Select(book => new BookReadDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    Genre = book.Genre,
                    Language = book.Language,
                    Format = book.Format,
                    ISBN = book.ISBN,
                    Publisher = book.Publisher,
                    Price = book.Price,
                    Stock = book.Stock,
                    Rating = book.Rating,
                    PublicationDate = book.PublicationDate
                })
                .ToListAsync();
        }

        public async Task<BookReadDto> GetBookByIdAsync(Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return null;

            return new BookReadDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Genre = book.Genre,
                Language = book.Language,
                Format = book.Format,
                ISBN = book.ISBN,
                Publisher = book.Publisher,
                Price = book.Price,
                Stock = book.Stock,
                Rating = book.Rating,
                PublicationDate = book.PublicationDate
            };
        }

        public async Task AddBookAsync(BookCreateDto bookCreateDto)
        {
            var book = new Entities.Book
            {
                Id = Guid.NewGuid(),
                Title = bookCreateDto.Title,
                Author = bookCreateDto.Author,
                Genre = bookCreateDto.Genre,
                Language = bookCreateDto.Language,
                Format = bookCreateDto.Format,
                ISBN = bookCreateDto.ISBN,
                Publisher = bookCreateDto.Publisher,
                Price = bookCreateDto.Price,
                Stock = bookCreateDto.Stock,
                PublicationDate = bookCreateDto.PublicationDate,
                Description = bookCreateDto.Description
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBookAsync(Guid id, BookUpdateDto bookUpdateDto)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return;

            book.Title = bookUpdateDto.Title;
            book.Author = bookUpdateDto.Author;
            book.Genre = bookUpdateDto.Genre;
            book.Language = bookUpdateDto.Language;
            book.Format = bookUpdateDto.Format;
            book.Publisher = bookUpdateDto.Publisher;
            book.Price = bookUpdateDto.Price;
            book.Stock = bookUpdateDto.Stock;
            book.PublicationDate = bookUpdateDto.PublicationDate;
            book.Description = bookUpdateDto.Description;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteBookAsync(Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
    }
}
