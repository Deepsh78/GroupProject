using GroupApi.CommonDomain;
using GroupApi.DTOs.Carts;

namespace GroupApi.Services.Carts
{
    public interface ICartService
    {
        Task<GenericResponse<IEnumerable<CartDto>>> GetCartByMemberAsync(Guid memberId);
        Task<GenericResponse<CartDto>> AddToCartAsync(Guid bookId, int quantity);
        Task<GenericResponse<CartDto>> UpdateCartItemAsync(Guid cartItemId, int quantity);
        Task<GenericResponse<CartDto>> RemoveFromCartAsync(Guid cartItemId);
        Task<GenericResponse<IEnumerable<CartDto>>> GetCartForCurrentUserAsync();
    }

}
