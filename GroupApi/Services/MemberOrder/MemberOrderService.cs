using Acb.Core.Extensions;
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
            var memberId = _currentUserService.UserId;

            // Apply discount
            var discountResult = await _discountService.GetDiscountAsync(cartItems);
            if (!discountResult.IsSuccess)
                return new ErrorModel(HttpStatusCode.BadRequest, "Error calculating discount");

            var totalAmount = discountResult.Data;

            // Create the order
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                MemberId = memberId,
                OrderDate = DateTime.UtcNow,
                Status = "Placed",
                TotalAmount = totalAmount,
                ClaimCode = GenerateClaimCode() // Generate claim code
            };

            await _orderRepo.AddAsync(order);

            // Add order items
            foreach (var cartItem in cartItems)
            {
                // Load the book details to get BookName and Price
                var book = await _bookRepo.TableNoTracking
                    .FirstOrDefaultAsync(b => b.BookId == cartItem.BookId);

                if (book == null)
                    return new ErrorModel(HttpStatusCode.NotFound, $"Book with ID {cartItem.BookId} not found");

                var orderItem = new OrderItem
                {
                    OrderItemId = Guid.NewGuid(),
                    OrderId = order.OrderId,
                    BookId = cartItem.BookId,
                    Quantity = cartItem.Quantity,
                    Price = book.Price // Ensure Price is set from Book entity
                };

                // Calculate TotalPrice for each OrderItem (Price * Quantity)
                orderItem.TotalPrice = orderItem.Price * cartItem.Quantity;

                await _orderItemRepo.AddAsync(orderItem);
            }

            await _orderRepo.SaveChangesAsync();

            // Prepare OrderDto for the response
            var orderDto = new OrderDto
            {
                OrderId = order.OrderId,
                MemberId = order.MemberId,
                OrderDate = order.OrderDate,
                Status = order.Status,
                ClaimCode = order.ClaimCode,
                OrderItems = cartItems.Select(ci => new OrderItemDto
                {
                    BookId = ci.BookId,
                    Quantity = ci.Quantity,
               
                    BookName = ci.BookName, // This will be automatically filled from the database during order item creation
                    TotalPrice = (decimal)(ci.Price * ci.Quantity)  // Automatically calculate the TotalPrice here
                }).ToList()
            };

            return new GenericResponse<OrderDto> { Data = orderDto };
        }


        private string GenerateClaimCode()
        {
            return "CLAIM-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
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
