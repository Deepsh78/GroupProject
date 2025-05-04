using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupApi.Entities.Books
{
    [Table("Book_Category")]
    public class BookCategory
    {
        [Key]
       public Guid BookCtegoryId { get; set; } // Primary Key
        public Guid CategoryId { get; set; } // Foreign Key for Category

       
        public Guid BookId { get; set; } // Foreign Key for Book

        // Navigation properties
        public Category Category { get; set; }
        public Book Book { get; set; }
    }
}
