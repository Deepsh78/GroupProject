using GroupApi.Constraint;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupApi.Entities
{
    [Table("User")]

    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GenderType Gender { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime RegistrData { get; set; } = DateTime.UtcNow;

        public bool IsActive{ get; set; } 

    }
}
