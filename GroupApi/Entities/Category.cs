using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupApi.Entities
{
    [Table("Category")]
    public class Category
    {
        [Key]
        public Guid CategoryId { get; set; } // Primary Key

        public string Name { get; set; }
    }
}
