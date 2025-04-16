using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupApi.Entities
{
    [Table("Member")]
    public class Member
    {
        [Key]
        public Guid MemberId { get; set; } // Primary Key

        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int OrderCount { get; set; }
    }
}
