using Acb.Core.Extensions;
using GroupApi.CommonDomain;
using GroupApi.DTOs.Carts;
using GroupApi.DTOs.MemberOrder;
using GroupApi.DTOs.Orders;
using GroupApi.Entities;
using GroupApi.Entities.Auth;
using GroupApi.Entities.Books;
using GroupApi.Entities.Oders;
using GroupApi.Entities.Orders;
using GroupApi.GenericClasses;
using GroupApi.Services.CurrentUser;
using GroupApi.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Threading.Tasks;

namespace GroupApi.Services.MemberOrder
{
    public class MemberOrderService : IMemberOrderService
    {
        private readonly IGenericRepository<Order> _orderRepo;
        private readonly IGenericRepository<Book> _bookRepo;
        private readonly IGenericRepository<OrderItem> _orderItemRepo;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMemberDiscountService _discountService;
        private readonly IGenericRepository<ClaimCode> _claimCodeRepo;
        private readonly IEmailService _emailService;
        private readonly IGenericRepository<CartItem> _cartItemRepo;
        private readonly IGenericRepository<Cart> _cartRepo;
        private readonly IGenericRepository<Member> _memberRepo;
        private readonly IGenericRepository<Publisher> _publisherRepo;

        public MemberOrderService(
            IGenericRepository<Order> orderRepo,
            IGenericRepository<OrderItem> orderItemRepo,
            ICurrentUserService currentUserService,
            IMemberDiscountService discountService,
            IGenericRepository<ClaimCode> claimCodeRepo,
            IGenericRepository<Book> bookRepo,
            IEmailService emailService, IGenericRepository<CartItem> cartItemRepo,
            IGenericRepository<Cart> cartRepo, IGenericRepository<Member> memberRepo
            )
        {
            _orderRepo = orderRepo;
            _orderItemRepo = orderItemRepo;
            _currentUserService = currentUserService;
            _discountService = discountService;
            _claimCodeRepo = claimCodeRepo;
            _bookRepo = bookRepo;
            _emailService = emailService;
            _cartItemRepo = cartItemRepo;
            _cartRepo = cartRepo;
            _memberRepo = memberRepo;
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
                    TotalAmount = totalAmount,
                    ClaimCode = GenerateClaimCode()
                };

                await _orderRepo.AddAsync(order);

                foreach (var cartItem in cartItems)
                {
                    var book = await _bookRepo.TableNoTracking
                        .FirstOrDefaultAsync(b => b.BookId == cartItem.BookId);

                    if (book == null)
                        return new ErrorModel(HttpStatusCode.NotFound, "Book not found");

                    var orderItem = new OrderItem
                    {
                        OrderItemId = Guid.NewGuid(),
                        OrderId = order.OrderId,
                        BookId = cartItem.BookId,
                        Quantity = cartItem.Quantity,
                        Price = book.Price
                    };
                    await _orderItemRepo.AddAsync(orderItem);
                }

                await _orderRepo.SaveChangesAsync();

                var memberEmail = _currentUserService.UserEmail;
                await _emailService.SendClaimCodeWithBillAsync(
                   memberEmail,
                   order.ClaimCode,
                   order.TotalAmount,
                   order.OrderId
                );

                // ✅ Delete Cart after placing the order
                var cart = await _cartRepo.Table
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.MemberId == memberId);

                if (cart != null)
                {
                    _cartItemRepo.DeleteRange(cart.CartItems);
                    _cartRepo.Delete(cart);
                    await _cartRepo.SaveChangesAsync();
                }

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
                        BookName = _bookRepo.TableNoTracking
                            .FirstOrDefault(b => b.BookId == ci.BookId)?.BookName,
                        Price = _bookRepo.TableNoTracking
                            .FirstOrDefault(b => b.BookId == ci.BookId)?.Price ?? 0,
                        TotalPrice = (ci.Quantity * (_bookRepo.TableNoTracking
                            .FirstOrDefault(b => b.BookId == ci.BookId)?.Price ?? 0))
                    }).ToList()
                };

                return new GenericResponse<OrderDto> { Data = orderDto };
            }
            catch
            {
                throw;
            }
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
        public async Task<GenericResponse<List<OrderDto>>> GetMyOrdersAsync()
        {
            var memberId = _currentUserService.UserId;

            var orders = await _orderRepo.TableNoTracking
                .Where(o => o.MemberId == memberId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var orderDtos = orders.Select(order => new OrderDto
            {
                OrderId = order.OrderId,
                MemberId = order.MemberId,
                OrderDate = order.OrderDate,
                Status = order.Status,
                ClaimCode = order.ClaimCode,
            }).ToList();

            return new GenericResponse<List<OrderDto>> { Data = orderDtos };
        }


    }
}
