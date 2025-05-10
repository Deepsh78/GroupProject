using GroupApi.Constants;
using GroupApi.Constraint;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GroupApi.Entities.Auth
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public GenderType Gender { get; set; }
        public RoleType Role { get; set; } = RoleType.User; 
    }
}
