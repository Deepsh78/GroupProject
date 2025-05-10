using GroupApi.CommonDomain;
using GroupApi.DTOs.Books;
using GroupApi.Entities.Books;
using GroupApi.Pagination;

namespace GroupApi.Services.Interface
{
    public interface IBookService
    {
        Task<GenericResponse<IEnumerable<BookReadDto>>> GetAllAsync();
        Task<GenericResponse<BookReadDto?>> GetByIdAsync(Guid id);
        Task<GenericResponse<BookReadDto>> CreateAsync(BookCreateDto dto);
        Task<GenericResponse<BookReadDto?>> UpdateAsync(Guid id, BookUpdateDto dto);
        Task<Response> DeleteAsync(Guid id);
        Task<PaginatedList<BookReadDto>> GetFilteredBooksAsync(BookFilterDto filter);
        Task<BookDetailDto?> GetBookDetailAsync(Guid id);
        Task AddBookmarkAsync(Guid memberId, Guid bookId);
    }


}
