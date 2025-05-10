using GroupApi.CommonDomain;
using GroupApi.DTOs.Carts;
using GroupApi.Entities.Oders;
using GroupApi.Entities.Orders;
using GroupApi.Entities;
using GroupApi.GenericClasses;
using GroupApi.Services.CurrentUser;
using GroupApi.Services.Interface;
using System.Net;

namespace GroupApi.Services.Discounts
{
    public class MemberDiscountService : IMemberDiscountService
    {
        private readonly IGenericRepository<Order> _orderRepo;
        private readonly IGenericRepository<OrderItem> _orderItemRepo;
        private readonly IGenericRepository<CartItem> _cartItemRepo;
        private readonly IGenericRepository<ClaimCode> _claimCodeRepo;
        private readonly ICurrentUserService _currentUserService;

        public MemberDiscountService(
            IGenericRepository<Order> orderRepo,
            IGenericRepository<OrderItem> orderItemRepo,
            IGenericRepository<CartItem> cartItemRepo,
            IGenericRepository<ClaimCode> claimCodeRepo,
            ICurrentUserService currentUserService)
        {
            _orderRepo = orderRepo;
            _orderItemRepo = orderItemRepo;
            _cartItemRepo = cartItemRepo;
            _claimCodeRepo = claimCodeRepo;
            _currentUserService = currentUserService;
        }

        public async Task<GenericResponse<decimal>> GetDiscountAsync(Guid memberId, List<CartItemDto> cartItems)
        {
            var totalBooks = cartItems.Sum(ci => ci.Quantity);
            decimal discount = 0;

            if (totalBooks >= 5)
            {
                discount = 0.05m;
            }

            var successfulOrders = await _orderRepo.TableNoTracking
                .Where(o => o.MemberId == memberId && o.Status == "Completed")
                .CountAsync();

            if (successfulOrders >= 10)
            {
                discount = Math.Max(discount, 0.10m); // If the member has 10+ successful orders, apply the stackable discount
            }

            decimal totalAmount = cartItems.Sum(ci => ci.Quantity * ci.Price);
            decimal discountedAmount = totalAmount * (1 - discount);

            return new GenericResponse<decimal> { Data = discountedAmount };
        }

        public async Task<GenericResponse<string>> GenerateClaimCodeAsync(Guid memberId)
        {
            var claimCode = "CLAIM-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

            var order = await _orderRepo.TableNoTracking
                .FirstOrDefaultAsync(o => o.MemberId == memberId && o.Status == "Placed");

            if (order == null)
                return new ErrorModel(HttpStatusCode.NotFound, "No order placed for the member.");

            var claim = new ClaimCode
            {
                ClaimCodeId = Guid.NewGuid(),
                Code = claimCode,
                OrderId = order.OrderId,
                CreatedAt = DateTime.UtcNow
            };

            await _claimCodeRepo.AddAsync(claim);
            await _orderRepo.SaveChangesAsync();

            return new GenericResponse<string> { Data = claimCode };
        }

        public async Task<GenericResponse<OrderDto>> PlaceOrderAsync(List<CartItemDto> cartItems)
        {
            var memberId = _currentUserService.UserId;
            var discountResult = await GetDiscountAsync(memberId, cartItems);

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
                    Price = cartItem.Price
                };
                await _orderItemRepo.AddAsync(orderItem);
            }

            await _orderRepo.SaveChangesAsync();

            var claimCodeResult = await GenerateClaimCodeAsync(memberId);

            if (!claimCodeResult.IsSuccess)
                return new ErrorModel(HttpStatusCode.BadRequest, "Failed to generate claim code");

            var orderDto = new OrderDto
            {
                OrderId = order.OrderId,
                MemberId = order.MemberId,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                ClaimCode = claimCodeResult.Data,
                OrderItems = cartItems.Select(ci => new OrderItemDto
                {
                    BookId = ci.BookId,
                    Quantity = ci.Quantity,
                    Price = ci.Price
                }).ToList()
            };

            return orderDto;
        }
    }
}
