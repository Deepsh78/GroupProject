using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupApi.Entities.Books
{
    [Table("Book_Format")]
    public class BookFormat
    {
        [Key]
        [Column(Order = 0)]
        public Guid FormatId { get; set; } // Foreign Key for Format

        [Key]
        [Column(Order = 1)]
        public Guid BookId { get; set; } // Foreign Key for Book

        // Navigation properties
        public Format Format { get; set; }
        public Book Book { get; set; }
    }
}
