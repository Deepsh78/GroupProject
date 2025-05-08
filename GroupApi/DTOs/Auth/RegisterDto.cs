using GroupApi.Constraint;
using System.ComponentModel.DataAnnotations;

namespace GroupApi.DTOs.Auth
{
    public class RegisterDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required, MinLength(6)]
        public string Password { get; set; }
        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; }
        [Required]
        public GenderType Gender { get; set; }
    }
}
