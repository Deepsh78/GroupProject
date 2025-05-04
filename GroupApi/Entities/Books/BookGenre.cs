using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupApi.Entities.Books
{
    [Table("Book_Genre")]
    public class BookGenre
    {
        [Key]
        [Column(Order = 0)]
        public Guid BookId { get; set; } // Foreign Key for Book

        [Key]
        [Column(Order = 1)]
        public Guid GenreId { get; set; } // Foreign Key for Genre

        // Navigation properties
        public Book Book { get; set; }
        public Genre Genre { get; set; }
    }
}