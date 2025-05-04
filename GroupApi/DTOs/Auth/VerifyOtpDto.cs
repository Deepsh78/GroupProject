// GroupApi.DTOs.Auth/VerifyOtpDto.cs
namespace GroupApi.DTOs.Auth
{
    public class VerifyOtpDto
    {
        public string Email { get; set; }
        public string Otp { get; set; }
    }
}