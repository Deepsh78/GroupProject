using System.ComponentModel.DataAnnotations;

namespace GroupApi.DTOs.Auth
{
    public class ResendOtpDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Purpose { get; set; }
    }
}
