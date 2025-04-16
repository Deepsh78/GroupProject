using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupApi.Entities
{
    [Table("Cart")]
    public class Cart
    {
        [Key]
        public Guid CartId { get; set; } // Primary Key

        // Foreign Key for Member
        public Guid MemberId { get; set; }
        public Member Member { get; set; }
    }
}
