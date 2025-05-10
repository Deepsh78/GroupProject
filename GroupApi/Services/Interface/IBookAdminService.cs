using GroupApi.CommonDomain;
using GroupApi.DTOs.Books;

namespace GroupApi.Services.Interface
{
    public interface IBookAdminService
    {
        Task<GenericResponse<IEnumerable<BookReadDto>>> GetAllAsync();
        Task<GenericResponse<BookReadDto?>> GetByIdAsync(Guid id);
        Task<GenericResponse<BookReadDto>> CreateAsync(BookCreateDto dto, string adminId);
        Task<GenericResponse<BookReadDto?>> UpdateAsync(Guid id, BookUpdateDto dto, string adminId);
        Task<Response> DeleteAsync(Guid id, string adminId);
        Task<GenericResponse<BookReadDto>> UpdateStockAsync(Guid id, UpdateStockDto dto, string adminId);
    }
}