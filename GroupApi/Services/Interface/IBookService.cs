using GroupApi.CommonDomain;
using GroupApi.DTOs.Books;
using GroupApi.Entities.Books;

namespace GroupApi.Services.Interface
{
    public interface IBookService
    {
        Task<GenericResponse<IEnumerable<BookReadDto>>> GetAllAsync();
        Task<GenericResponse<BookReadDto?>> GetByIdAsync(Guid id);
        Task<GenericResponse<BookReadDto>> CreateAsync(BookCreateDto dto);
        Task<GenericResponse<BookReadDto?>> UpdateAsync(Guid id, BookUpdateDto dto);
        Task<Response> DeleteAsync(Guid id);
    }


}
