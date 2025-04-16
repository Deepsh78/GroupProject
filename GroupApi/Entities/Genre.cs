using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupApi.Entities
{
    [Table("Genre")]
    public class Genre
    {
        [Key]
        public Guid GenreId { get; set; } // Primary Key

        public string Name { get; set; }
    }
}
