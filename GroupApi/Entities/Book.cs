using DocumentFormat.OpenXml.Bibliography;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupApi.Entities
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
    }

}
