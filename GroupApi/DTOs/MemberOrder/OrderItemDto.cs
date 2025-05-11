namespace GroupApi.DTOs.MemberOrder
{
    public class OrderItemDto
    {
        public Guid OrderItemId { get; set; }
        public Guid OrderId { get; set; }
        public Guid BookId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

}
