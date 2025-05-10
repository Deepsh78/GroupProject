// GroupApi.Services.Interface/IDiscountService.cs
using GroupApi.CommonDomain;
using GroupApi.DTOs.Discount;

namespace GroupApi.Services.Interface
{
    public interface IDiscountService
    {
        Task<GenericResponse<IEnumerable<DiscountReadDto>>> GetAllAsync();
        Task<GenericResponse<DiscountReadDto?>> GetByIdAsync(Guid id);
        Task<GenericResponse<DiscountReadDto>> CreateAsync(DiscountCreateDto dto, string adminId);
        Task<GenericResponse<DiscountReadDto?>> UpdateAsync(Guid id, DiscountUpdateDto dto, string adminId);
        Task<Response> DeleteAsync(Guid id, string adminId);
        Task<GenericResponse<IEnumerable<DiscountReadDto>>> GetActiveDiscountsAsync();
        Task<GenericResponse<IEnumerable<DiscountReadDto>>> GetOnSaleDiscountsAsync();
    }
}