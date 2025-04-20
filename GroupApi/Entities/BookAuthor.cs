using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupApi.Entities
{
    [Table("Book_Author")]
    public class BookAuthor
    {
        [Key]
       public Guid BookAuthorId { get; set; } // Primary Key
        public Guid AuthorId { get; set; } // Foreign Key for Author

       
        public Guid BookId { get; set; } // Foreign Key for Book

        // Navigation properties
        public Author Author { get; set; }
        public Book Book { get; set; }
    }
}