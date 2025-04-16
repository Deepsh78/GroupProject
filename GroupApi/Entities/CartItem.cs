using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupApi.Entities
{
    [Table("Cart_Item")]
    public class CartItem
    {
        [Key]
        public Guid CartItemId { get; set; } // Primary Key

        // Foreign Key for Cart
        public Guid CartId { get; set; }
        public Cart Cart { get; set; }

        // Foreign Key for Book
        public Guid BookId { get; set; }
        public Book Book { get; set; }

        public int Quantity { get; set; }
    }
}
