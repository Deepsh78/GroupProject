using GroupApi.CommonDomain;
using GroupApi.DTOs.Carts;
using GroupApi.Entities.Oders;
using GroupApi.Entities.Orders;
using GroupApi.GenericClasses;
using GroupApi.Services.CurrentUser;
using GroupApi.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Threading.Tasks;

namespace GroupApi.Services.Discounts
{
    public class MemberDiscountService : IMemberDiscountService
    {
        private readonly IGenericRepository<Order> _orderRepo;
        private readonly IGenericRepository<ClaimCode> _claimCodeRepo;
        private readonly ICurrentUserService _currentUserService;

        public MemberDiscountService(
            IGenericRepository<Order> orderRepo,
            IGenericRepository<ClaimCode> claimCodeRepo,
            ICurrentUserService currentUserService)
        {
            _orderRepo = orderRepo;
            _claimCodeRepo = claimCodeRepo;
            _currentUserService = currentUserService;
        }

        public async Task<GenericResponse<decimal>> GetDiscountAsync( List<CartItemDto> cartItems)
        {
            var memberId = _currentUserService.UserId;
            decimal discount = 0;
            var totalBooks = cartItems.Sum(ci => ci.Quantity);

            if (totalBooks >= 5)
            {
                discount = 0.05m;
            }

            var successfulOrders = await _orderRepo.TableNoTracking
                .Where(o => o.MemberId == memberId && o.Status == "Completed")
                .CountAsync();

            if (successfulOrders >= 10)
            {
                discount = Math.Max(discount, 0.10m); 
            }

            decimal totalAmount = (decimal)cartItems.Sum(ci => ci.Quantity * ci.Price);
            decimal discountedAmount = totalAmount * (1 - discount);

            return new GenericResponse<decimal> { Data = discountedAmount };
        }

        public async Task<GenericResponse<string>> GenerateClaimCodeAsync(
            )
        {
            var memberId = _currentUserService.UserId;
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
    }
}
