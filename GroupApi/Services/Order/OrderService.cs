using GroupApi.CommonDomain;
using GroupApi.Constants;
using GroupApi.Data;
using GroupApi.DTOs.Orders;
using GroupApi.Entities.Auth;
using GroupApi.Entities.Oders;
using GroupApi.GenericClasses;
using GroupApi.Services.Interface;
using GroupApi.Services.WebSocket; // Add this for WebSocketService
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace GroupApi.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IGenericRepository<Order> _orderRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly WebSocketService _webSocketService; // Add WebSocketService dependency

        public OrderService(
            IGenericRepository<Order> orderRepo,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            WebSocketService webSocketService) // Inject WebSocketService
        {
            _orderRepo = orderRepo;
            _userManager = userManager;
            _context = context;
            _webSocketService = webSocketService;
        }

        public async Task<GenericResponse<bool>> ProcessClaimCodeAsync(ProcessClaimCodeDto dto, string staffId)
        {
            // Verify staff role
            var staff = await _userManager.FindByIdAsync(staffId);
            if (staff == null || staff.Role != RoleType.Staff)
                return new ErrorModel(HttpStatusCode.Forbidden, "Only staff can process claim codes");

            // Find the order with its items and books
            var order = await _orderRepo.Table
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
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

            // Prepare and broadcast book details
            var bookNames = order.OrderItems.Select(oi => oi.Book.BookName).ToList();
            var message = new
            {
                OrderId = order.OrderId,
                Books = bookNames,
                Message = $"Order {order.OrderId} has been successfully fulfilled with {bookNames.Count} book(s)."
            };
            await _webSocketService.BroadcastMessageAsync(JsonSerializer.Serialize(message));

            return new GenericResponse<bool> { Data = true };
        }
    }
}