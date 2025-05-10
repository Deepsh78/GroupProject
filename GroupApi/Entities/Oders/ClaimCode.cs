
using GroupApi.Entities.Oders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupApi.Entities.Orders
{
    [Table("ClaimCode")]
    public class ClaimCode
    {
        [Key]
        public Guid ClaimCodeId { get; set; }
        public string Code { get; set; } // Unique claim code
        public Guid OrderId { get; set; } // Foreign Key for Order
        public Order Order { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime? UsedAt { get; set; }
    }
}