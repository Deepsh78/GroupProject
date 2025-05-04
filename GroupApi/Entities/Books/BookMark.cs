using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupApi.Entities.Books
{
    [Table("BookMark")]
    public class BookMark
    {
        [Key]
        public Guid BookMarkId { get; set; } // Primary Key

        // Foreign Key for Book
        public Guid BookId { get; set; }
        public Book Book { get; set; }

        // Foreign Key for Member
        public Guid MemberId { get; set; }
        public Member Member { get; set; }
    }
}
