using GroupApi.Entities.Books;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupApi.Entities.Discount
{
    [Table("Discount")]
    public class Discount
    {
        [Key]
        public Guid DiscountId { get; set; }

        public Guid BookId { get; set; }
        public Book Book { get; set; }

        [Range(0, 100)]
        public decimal Percentage { get; set; }

        public bool OnSale { get; set; } = false; 

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsActive => DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate;
    }
}
