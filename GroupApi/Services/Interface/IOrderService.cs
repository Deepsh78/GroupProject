using GroupApi.CommonDomain;
using GroupApi.DTOs.Orders;

namespace GroupApi.Services.Interface
{
    public interface IOrderService
    {
        Task<GenericResponse<ClaimCodeDto>> GenerateClaimCodeAsync(Guid orderId);
        Task<GenericResponse<ClaimCodeDto>> ProcessClaimCodeAsync(ProcessClaimCodeDto dto, Guid staffId);
    }
}