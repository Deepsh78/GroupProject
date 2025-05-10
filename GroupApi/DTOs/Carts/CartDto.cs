namespace GroupApi.DTOs.Carts
{
    public class CartDto
    {
        public Guid CartId { get; set; } // Primary Key
        public Guid MemberId { get; set; } // Foreign Key for Member
        public List<CartItemDto>? CartItems { get; set; } // List of Cart Items
    }

}
