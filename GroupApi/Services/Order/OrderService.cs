using GroupApi.CommonDomain;
using GroupApi.Constants;
using GroupApi.Data;
using GroupApi.DTOs.Orders;
using GroupApi.Entities.Auth;
using GroupApi.Entities.Oders;
using GroupApi.GenericClasses;
using GroupApi.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Threading.Tasks;

namespace GroupApi.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IGenericRepository<Order> _orderRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public OrderService(
            IGenericRepository<Order> orderRepo,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _orderRepo = orderRepo;
            _userManager = userManager;
            _context = context;
        }

        public async Task<GenericResponse<bool>> ProcessClaimCodeAsync(ProcessClaimCodeDto dto, string staffId)
        {
            // Verify staff role
            var staff = await _userManager.FindByIdAsync(staffId);
            if (staff == null || staff.Role != RoleType.Staff)
                return new ErrorModel(HttpStatusCode.Forbidden, "Only staff can process claim codes");

            // Find the order
            var order = await _orderRepo.Table
                .FirstOrDefaultAsync(o => o.OrderId == dto.OrderId);

            if (order == null)
                return new ErrorModel(HttpStatusCode.NotFound, "Order not found");

            // Verify claim code
            if (order.ClaimCode != dto.ClaimCode)
                return new ErrorModel(HttpStatusCode.BadRequest, "Invalid claim code");

            // Check if order is already fulfilled
            if (order.Status == "Fulfilled")
                return new ErrorModel(HttpStatusCode.BadRequest, "Order is already fulfilled");

            // Update order status
            order.Status = "Fulfilled";
            _orderRepo.Update(order);
            await _orderRepo.SaveChangesAsync();

            return new GenericResponse<bool> { Data = true };
        }
    }
}