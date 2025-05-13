using GroupApi.CommonDomain;
using GroupApi.DTOs.Carts;
using GroupApi.DTOs.MemberOrder;

namespace GroupApi.Services.Interface
{
    public interface IMemberOrderService
    {
        Task<GenericResponse<OrderDto>> PlaceOrderAsync(List<CartItemDto> cartItems);

        Task<GenericResponse<string>> CancelOrderAsync(Guid orderId);
        Task<GenericResponse<List<OrderDto>>> GetMyOrdersAsync();

    }
}
