using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupApi.Entities.Books
{
    [Table("Book_Format")]
    public class BookFormat
    {
        [Key]
        public Guid BookFormatId { get; set; } // Primary Key
        public Guid FormatId { get; set; } // Foreign Key for Format

        public Guid BookId { get; set; } // Foreign Key for Book

        // Navigation properties
        public Format Format { get; set; }
        public Book Book { get; set; }
    }
}
