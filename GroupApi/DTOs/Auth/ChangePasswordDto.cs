// GroupApi.DTOs.Auth/ChangePasswordDto.cs
namespace GroupApi.DTOs.Auth
{
    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}