using GroupApi.Constants;
using GroupApi.Constraint;
using System.ComponentModel.DataAnnotations;

namespace GroupApi.DTOs.Auth
{
    public class RegisterUserDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        [Required]
        public GenderType Gender { get; set; }
        public RoleType Role { get; set; } = RoleType.User;
    }
}