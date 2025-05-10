using DocumentFormat.OpenXml.Bibliography;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupApi.Entities.Books
{
    [Table("Book")]
    public class Book
    {
        [Key]
        public Guid BookId { get; set; } // Primary Key

        public string BookName { get; set; }
        public string ISBN { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }

        // Foreign Key for Publisher
        public Guid PublisherId { get; set; }

        // Navigation property
        public Publisher Publisher { get; set; }
        public int Stock { get; set; } // To support availability filter
        public DateTime PublicationDate { get; set; } // For "New Releases"
        public DateTime CreatedAt { get; set; } // For "New Arrivals"
        public bool IsComingSoon { get; set; } // For "Coming Soon" tab
        public ICollection<BookAuthor> BookAuthors { get; set; }
        public ICollection<BookGenre> BookGenres { get; set; }
        public ICollection<BookFormat> BookFormats { get; set; }
        public ICollection<BookCategory> BookCategories { get; set; }

    }

}
