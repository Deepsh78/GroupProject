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

        public OrderService(
            IGenericRepository<ClaimCode> claimCodeRepo,
            IGenericRepository<Order> orderRepo,
            IGenericRepository<Member> memberRepo)
        {
            _claimCodeRepo = claimCodeRepo;
            _orderRepo = orderRepo;
            _memberRepo = memberRepo;
        }

        public async Task<GenericResponse<ClaimCodeDto>> GenerateClaimCodeAsync(Guid orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null)
                return new ErrorModel(HttpStatusCode.NotFound, "Order not found");

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