using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupApi.Entities
{
    [Table("Format")]
    public class Format
    {
        [Key]
        public Guid FormatId { get; set; } // Primary Key

        public string Name { get; set; }
    }
}