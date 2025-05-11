using GroupApi.CommonDomain;
using GroupApi.DTOs.Carts;

namespace GroupApi.Services.Interface
{
    public interface ICartService
    {
        // Get all items in the cart for a specific member
        Task<GenericResponse<IEnumerable<CartDto>>> GetAllAsync();

        // Get a specific cart by member ID
        Task<GenericResponse<CartDto?>> GetByIdAsync();

        // Add a book to the cart
        Task<GenericResponse<CartDto>> AddAsync(Guid bookId, int quantity);

        // Update the quantity of a book in the cart
        Task<GenericResponse<CartDto>> UpdateAsync(Guid cartItemId, int quantity);

        // Remove a book from the cart
        Task<GenericResponse<CartDto>> RemoveAsync(Guid cartItemId);
    }

}
