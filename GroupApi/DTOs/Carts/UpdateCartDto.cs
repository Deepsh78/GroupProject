namespace GroupApi.DTOs.Carts
{
    public class UpdateCartDto
    {
        public Guid CartItemId { get; set; }
        public int Quantity { get; set; }
    }
}
