using System;

namespace GroupApi.DTOs.Orders
{
    public class OrderDto
    {
        public Guid OrderId { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public int BookCount { get; set; }
    }
}
