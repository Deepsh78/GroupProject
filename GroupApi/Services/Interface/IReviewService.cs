using GroupApi.CommonDomain;
using GroupApi.DTOs.Reviews;

namespace GroupApi.Services.Interface
{
    public interface IReviewService
    {
        Task<GenericResponse<IEnumerable<ReviewDto>>> GetAllAsync();
        Task<GenericResponse<ReviewDto?>> GetByIdAsync(Guid id);
        Task<GenericResponse<ReviewDto>> CreateAsync(ReviewCreateDto dto);
        Task<GenericResponse<ReviewDto?>> UpdateAsync(Guid id, UpdateReviewDto dto);
        Task<Response> DeleteAsync(Guid id);
    }


}
