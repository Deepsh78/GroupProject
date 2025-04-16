using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupApi.Entities
{
    [Table("Review")]
    public class Review
    {
        [Key]
        public Guid ReviewId { get; set; } // Primary Key

        // Foreign Key for Member
        public Guid MemberId { get; set; }
        public Member Member { get; set; }

        // Foreign Key for Book
        public Guid BookId { get; set; }
        public Book Book { get; set; }

        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
