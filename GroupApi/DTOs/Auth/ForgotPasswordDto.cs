using System.ComponentModel.DataAnnotations;

namespace GroupApi.DTOs.Auth
{
    public class ForgotPasswordDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
