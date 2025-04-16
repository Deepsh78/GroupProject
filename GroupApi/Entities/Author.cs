using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupApi.Entities
{
    [Table("Author")]
    public class Author
    {
        [Key]
        public Guid AuthorId { get; set; } // Primary Key

        public string Name { get; set; }
    }
}
