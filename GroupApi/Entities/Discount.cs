using GroupApi.Entities.Books;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupApi.Entities
{
    [Table("Discount")]
    public class Discount
    {
        [Key]
        public Guid DiscountId { get; set; } // Primary Key

        // Foreign Key for Book
        public Guid BookId { get; set; }
        public Book Book { get; set; }

        public decimal Percentage { get; set; }
        public bool OnSale { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? StartDate { get; set; }
    }
}
