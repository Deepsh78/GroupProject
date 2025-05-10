namespace GroupApi.DTOs.Carts
{
    public class CartItemDto
    {
        public Guid CartItemId { get; set; } // Primary Key
        public Guid CartId { get; set; } // Foreign Key for Cart
        public Guid BookId { get; set; } // Foreign Key for Book
        public string? BookName { get; set; } // Book name (for read DTO)
        public int Quantity { get; set; } // Quantity of the book in the cart
        public decimal? Price { get; set; } // Book price

        public decimal TotalPrice { get; set; }
    }

}