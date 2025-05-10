namespace GroupApi.DTOs.Carts
{
    public class AddToCartDto
    {
        public Guid BookId { get; set; }
        public int Quantity { get; set; }
    }
}
