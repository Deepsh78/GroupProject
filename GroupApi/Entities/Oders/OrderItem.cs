using GroupApi.Entities.Books;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupApi.Entities.Oders
{
    [Table("Order_Item")]
    public class OrderItem
    {
        [Key]
        public Guid OrderItemId { get; set; } // Primary Key

        // Foreign Key for Book
        public Guid BookId { get; set; }
        public Book Book { get; set; }

        // Foreign Key for Order
        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        public int Quantity { get; set; }
    }
}
