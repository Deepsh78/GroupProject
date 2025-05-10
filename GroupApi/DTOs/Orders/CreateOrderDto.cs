using System;
using System.Collections.Generic;

namespace GroupApi.DTOs.Orders
{
    public class CreateOrderDto
    {
        public Guid MemberId { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
    }

    public class OrderItemDto
    {
        public Guid BookId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
