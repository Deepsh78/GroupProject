namespace GroupApi.DTOs.MemberOrder
{
    public class OrderItemDto
    {
      public string BookName { get; set; }
        public Guid BookId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
    }

}
