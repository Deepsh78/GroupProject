using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupApi.Entities
{
    [Table("Order")]
    public class Order
    {
        [Key]
        public Guid OrderId { get; set; } // Primary Key

        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }

        // Foreign Key for Member
        public Guid MemberId { get; set; }
        public Member Member { get; set; }
    }
}
