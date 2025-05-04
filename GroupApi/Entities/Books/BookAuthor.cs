using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupApi.Entities.Books
{
    [Table("Book_Author")]
    public class BookAuthor
    {
        [Key]
        [Column(Order = 0)]
        public Guid AuthorId { get; set; } // Foreign Key for Author

        [Key]
        [Column(Order = 1)]
        public Guid BookId { get; set; } // Foreign Key for Book

        // Navigation properties
        public Author Author { get; set; }
        public Book Book { get; set; }
    }
}