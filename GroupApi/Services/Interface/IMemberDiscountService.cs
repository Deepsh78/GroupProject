using GroupApi.CommonDomain;
using GroupApi.DTOs.Carts;

namespace GroupApi.Services.Interface
{
   
        public interface IMemberDiscountService
        {
            Task<GenericResponse<decimal>> GetDiscountAsync( List<CartItemDto> cartItems);
            Task<GenericResponse<string>> GenerateClaimCodeAsync();
        }
    

}
