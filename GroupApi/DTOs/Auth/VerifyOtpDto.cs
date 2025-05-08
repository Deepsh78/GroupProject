using System.ComponentModel.DataAnnotations;

namespace GroupApi.DTOs.Auth
{
    public class VerifyOtpDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string OTP { get; set; }
    }
}
