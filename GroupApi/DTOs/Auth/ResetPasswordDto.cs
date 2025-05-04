// GroupApi.DTOs.Auth/ResetPasswordDto.cs
namespace GroupApi.DTOs.Auth
{
    public class ResetPasswordDto
    {
        public string Email { get; set; }
        public string Otp { get; set; }
        public string NewPassword { get; set; }
    }
}