using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupApi.Entities.Books
{
    [Table("Book_Category")]
    public class BookCategory
    {
        [Key]
        [Column(Order = 0)]
        public Guid CategoryId { get; set; } // Foreign Key for Category

        [Key]
        [Column(Order = 1)]
        public Guid BookId { get; set; } // Foreign Key for Book

        // Navigation properties
        public Category Category { get; set; }
        public Book Book { get; set; }
    }
}
