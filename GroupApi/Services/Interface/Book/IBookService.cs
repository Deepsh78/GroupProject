using GroupApi.Dto.Book;

namespace GroupApi.Services.Interface.Book
{
    public interface IBookService
    {
        Task<IEnumerable<BookReadDto>> GetAllBooksAsync();
        Task<BookReadDto> GetBookByIdAsync(Guid id);
        Task AddBookAsync(BookCreateDto bookCreateDto);
        Task UpdateBookAsync(Guid id, BookUpdateDto bookUpdateDto);
        Task DeleteBookAsync(Guid id);
    }
}
