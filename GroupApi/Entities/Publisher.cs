using DocumentFormat.OpenXml.Bibliography;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupApi.Entities
{
    [Table("Publisher")]
    public class Publisher
    {
        [Key]
        public Guid PublisherId { get; set; } // Primary Key

        public string Name { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }

        // Navigation property for related books
        public ICollection<Book> Book { get; set; }
    }


}
