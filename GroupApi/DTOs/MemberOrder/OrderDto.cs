namespace GroupApi.DTOs.MemberOrder
{
    public class OrderDto
    {
        public Guid OrderId { get; set; }
        public Guid MemberId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        //public decimal TotalAmount { get; set; }
        public string ClaimCode { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
    }
}
