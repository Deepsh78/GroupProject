using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupApi.Entities.Oders
{
    [Table("Order")]
    public class Order
    {
        [Key]
        public Guid OrderId { get; set; } // Primary Key

        public string Status { get; set; } // "Pending", "Completed", "Cancelled"
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; } = 0;
        public decimal FinalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public int BookCount { get; set; }

        // Foreign Key for Member
        public Guid MemberId { get; set; }
        public Member Member { get; set; }

        // Navigation properties
        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<ClaimCode> ClaimCodes { get; set; }
    }
}
