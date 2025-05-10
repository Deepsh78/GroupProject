using GroupApi.CommonDomain;
using GroupApi.DTOs.Carts;

namespace GroupApi.Services.Interface
{
    public interface IMemberDiscountService
    {
        Task<GenericResponse<decimal>> GetDiscountAsync(Guid memberId);
        Task<GenericResponse<decimal>> ApplyDiscountAsync(Guid memberId, List<CartItemDto> cartItems);
    }
}
