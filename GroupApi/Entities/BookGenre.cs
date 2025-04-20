using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupApi.Entities
{
    [Table("Book_Genre")]
    public class BookGenre
    {
        [Key]
        public Guid BookGenreId { get; set; } // Primary Key
        public Guid BookId { get; set; } // Foreign Key for Book

        public Guid GenreId { get; set; } // Foreign Key for Genre

        // Navigation properties
        public Book Book { get; set; }
        public Genre Genre { get; set; }
    }
}