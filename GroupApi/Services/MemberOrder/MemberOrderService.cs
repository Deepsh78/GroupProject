using GroupApi.CommonDomain;
using GroupApi.DTOs.Carts;
using GroupApi.DTOs.MemberOrder;
using GroupApi.DTOs.Orders;
using GroupApi.Entities.Oders;
using GroupApi.Entities.Orders;
using GroupApi.GenericClasses;
using GroupApi.Services.CurrentUser;
using GroupApi.Services.Interface;
using System.Net;
using System.Threading.Tasks;

namespace GroupApi.Services.MemberOrder
{
    public class MemberOrderService : IMemberOrderService
    {
        private readonly IGenericRepository<Order> _orderRepo;
        private readonly IGenericRepository<OrderItem> _orderItemRepo;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMemberDiscountService _discountService;
        private readonly IGenericRepository<ClaimCode> _claimCodeRepo;

        public MemberOrderService(
            IGenericRepository<Order> orderRepo,
            IGenericRepository<OrderItem> orderItemRepo,
            ICurrentUserService currentUserService,
            IMemberDiscountService discountService,
            IGenericRepository<ClaimCode> claimCodeRepo)
        {
            _orderRepo = orderRepo;
            _orderItemRepo = orderItemRepo;
            _currentUserService = currentUserService;
            _discountService = discountService;
            _claimCodeRepo = claimCodeRepo;
        }

        public async Task<GenericResponse<OrderDto>> PlaceOrderAsync(List<CartItemDto> cartItems)
        {
            try
            {
                var memberId = _currentUserService.UserId;
                var discountResult = await _discountService.GetDiscountAsync(cartItems);
                if (!discountResult.IsSuccess)
                    return new ErrorModel(HttpStatusCode.BadRequest, "Error calculating discount");

                var totalAmount = discountResult.Data;

                var order = new Order
                {
                    OrderId = Guid.NewGuid(),
                    MemberId = memberId,
                    OrderDate = DateTime.UtcNow,
                    Status = "Placed",
                    TotalAmount = totalAmount
                };

                await _orderRepo.AddAsync(order);

                foreach (var cartItem in cartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderItemId = Guid.NewGuid(),
                        OrderId = order.OrderId,
                        BookId = cartItem.BookId,
                        Quantity = cartItem.Quantity,

                    };
                    await _orderItemRepo.AddAsync(orderItem);
                }

                await _orderRepo.SaveChangesAsync();

                var claimCode = "CLAIM-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

                var claim = new ClaimCode
                {
                    ClaimCodeId = Guid.NewGuid(),
                    Code = claimCode,
                    OrderId = order.OrderId,
                    CreatedAt = DateTime.UtcNow
                };

                await _claimCodeRepo.AddAsync(claim);
                await _orderRepo.SaveChangesAsync();

                var orderDto = new OrderDto
                {
                    OrderId = order.OrderId,
                    MemberId = order.MemberId,
                    OrderDate = order.OrderDate,
                    Status = order.Status,
                    TotalAmount = order.TotalAmount,
                    ClaimCode = claimCode,
                    OrderItems = cartItems.Select(ci => new OrderItemDto
                    {
                        BookId = ci.BookId,
                        Quantity = ci.Quantity,

                    }).ToList()
                };

                return new GenericResponse<OrderDto> { Data = orderDto };
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<GenericResponse<string>> CancelOrderAsync(Guid orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null || order.Status == "Fulfilled" || order.Status == "Cancelled")
                return new ErrorModel(HttpStatusCode.BadRequest, "Order cannot be cancelled");

            order.Status = "Cancelled"; // Update the status to "Cancelled"
            await _orderRepo.SaveChangesAsync();

            return new GenericResponse<string> { Data = "Order has been successfully cancelled" };
        }
    }
}
