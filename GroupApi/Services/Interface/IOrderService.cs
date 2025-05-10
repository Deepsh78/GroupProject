using GroupApi.CommonDomain;
using GroupApi.DTOs.Orders;

namespace GroupApi.Services.Interface
{
    public interface IOrderService
    {
        Task<GenericResponse<bool>> ProcessClaimCodeAsync(ProcessClaimCodeDto dto, string staffId);
    }
}