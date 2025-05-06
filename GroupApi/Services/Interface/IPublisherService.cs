using GroupApi.CommonDomain;
using GroupApi.DTOs.Publisher;

namespace GroupApi.Services.Interface
{
    public interface IPublisherService
    {
        Task<GenericResponse<IEnumerable<PublisherReadDto>>> GetAllAsync();
        Task<GenericResponse<PublisherReadDto?>> GetByIdAsync(Guid id);
        Task<GenericResponse<PublisherReadDto>> CreateAsync(PublisherCreateDto dto);
        Task<GenericResponse<PublisherReadDto?>> UpdateAsync(Guid id, PublisherUpdateDto dto);
        Task<Response> DeleteAsync(Guid id);
    }

}
