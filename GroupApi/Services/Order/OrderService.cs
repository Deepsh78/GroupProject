using GroupApi.CommonDomain;
using GroupApi.DTOs.Orders;
using GroupApi.Entities;
using GroupApi.Entities.Oders;
using GroupApi.Entities.Orders;
using GroupApi.GenericClasses;
using GroupApi.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Cryptography;

namespace GroupApi.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IGenericRepository<ClaimCode> _claimCodeRepo;
        private readonly IGenericRepository<Order> _orderRepo;
        private readonly IGenericRepository<Member> _memberRepo;
        private readonly IGenericRepository<OrderItem> _orderItemRepo;
        private readonly IEmailService _emailService;

        public OrderService(
            IGenericRepository<ClaimCode> claimCodeRepo,
            IGenericRepository<Order> orderRepo,
            IGenericRepository<Member> memberRepo,
            IGenericRepository<OrderItem> orderItemRepo,
            IEmailService emailService)
        {
            _claimCodeRepo = claimCodeRepo;
            _orderRepo = orderRepo;
            _memberRepo = memberRepo;
            _orderItemRepo = orderItemRepo;
            _emailService = emailService;
        }

        public async Task<GenericResponse<OrderDto>> CreateOrderAsync(CreateOrderDto dto)
        {
            var member = await _memberRepo.GetByIdAsync(dto.MemberId);
            if (member == null)
                return new ErrorModel<OrderDto>(HttpStatusCode.NotFound, "Member not found");

            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                Status = "Pending",
                MemberId = dto.MemberId,
                OrderDate = DateTime.UtcNow,
                BookCount = dto.OrderItems.Count,
                TotalAmount = dto.OrderItems.Sum(x => x.Price * x.Quantity)
            };

            // Calculate discounts
            decimal discountAmount = 0;
            if (order.BookCount >= 5)
            {
                discountAmount += order.TotalAmount * 0.05m; // 5% discount for 5+ books
            }

            if (member.OrderCount >= 10)
            {
                discountAmount += order.TotalAmount * 0.10m; // 10% stackable discount
            }

            order.DiscountAmount = discountAmount;
            order.FinalAmount = order.TotalAmount - discountAmount;

            await _orderRepo.AddAsync(order);
            await _orderRepo.SaveChangesAsync();

            // Generate claim code and send email
            var claimCode = await GenerateClaimCodeAsync(order.OrderId);
            if (claimCode.IsSuccess)
            {
                await _emailService.SendClaimCodeEmailAsync(member.Email, claimCode.Data.Code);
            }

            return new OrderDto
            {
                OrderId = order.OrderId,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                DiscountAmount = order.DiscountAmount,
                FinalAmount = order.FinalAmount,
                OrderDate = order.OrderDate,
                BookCount = order.BookCount
            };
        }

        public async Task<GenericResponse<OrderDto>> CancelOrderAsync(Guid orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null)
                return new ErrorModel<OrderDto>(HttpStatusCode.NotFound, "Order not found");

            if (order.Status != "Pending")
                return new ErrorModel<OrderDto>(HttpStatusCode.BadRequest, "Order cannot be cancelled as it's not in pending status");

            order.Status = "Cancelled";
            await _orderRepo.UpdateAsync(order);
            await _orderRepo.SaveChangesAsync();

            return new OrderDto
            {
                OrderId = order.OrderId,
                Status = order.Status
            };
        }

        public async Task<GenericResponse<ClaimCodeDto>> GenerateClaimCodeAsync(Guid orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null)
                return new ErrorModel<ClaimCodeDto>(HttpStatusCode.NotFound, "Order not found");

            // Generate a unique 8-character claim code
            var code = GenerateClaimCode();
            var claimCode = new ClaimCode
            {
                ClaimCodeId = Guid.NewGuid(),
                Code = code,
                OrderId = orderId,
                CreatedAt = DateTime.UtcNow
            };

            await _claimCodeRepo.AddAsync(claimCode);
            await _claimCodeRepo.SaveChangesAsync();

            return new ClaimCodeDto
            {
                ClaimCodeId = claimCode.ClaimCodeId,
                Code = claimCode.Code,
                OrderId = claimCode.OrderId,
                IsUsed = claimCode.IsUsed,
                CreatedAt = claimCode.CreatedAt
            };
        }

        public async Task<GenericResponse<ClaimCodeDto>> ProcessClaimCodeAsync(ProcessClaimCodeDto dto, Guid staffId)
        {
            var claimCode = await _claimCodeRepo.Table
                .FirstOrDefaultAsync(c => c.Code == dto.Code);

            if (claimCode == null)
                return new ErrorModel(HttpStatusCode.NotFound, "Claim code not found");

            if (claimCode.IsUsed)
                return new ErrorModel(HttpStatusCode.BadRequest, "Claim code already used");

            var order = await _orderRepo.GetByIdAsync(claimCode.OrderId);
            if (order == null)
                return new ErrorModel(HttpStatusCode.NotFound, "Order not found");

            // Update order status to fulfilled
            order.Status = "Fulfilled";
            _orderRepo.Update(order);

            // Mark claim code as used
            claimCode.IsUsed = true;
            claimCode.UsedAt = DateTime.UtcNow;
            _claimCodeRepo.Update(claimCode);

            await _claimCodeRepo.SaveChangesAsync();

            return new ClaimCodeDto
            {
                ClaimCodeId = claimCode.ClaimCodeId,
                Code = claimCode.Code,
                OrderId = claimCode.OrderId,
                IsUsed = claimCode.IsUsed,
                CreatedAt = claimCode.CreatedAt,
                UsedAt = claimCode.UsedAt
            };
        }

        private string GenerateClaimCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var bytes = new byte[8];
            RandomNumberGenerator.Fill(bytes);
            var result = new char[8];
            for (int i = 0; i < 8; i++)
            {
                result[i] = chars[bytes[i] % chars.Length];
            }
            return new string(result);
        }
    }
}