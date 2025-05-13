using GroupApi.CommonDomain;
using GroupApi.DTOs.Auth;

namespace GroupApi.Services.Interface
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterDto model);
        Task<bool> VerifyOtpAsync(VerifyOtpDto model);
        Task<string> LoginAsync(LoginDto model);
        Task<bool> ForgotPasswordAsync(ForgotPasswordDto model);
        Task<bool> VerifyPasswordResetOtpAsync(VerifyOtpDto model);
        Task<bool> ResetPasswordAsync(ResetPasswordDto model);
        Task<bool> AssignStaffRoleAsync(string userId);
        Task<bool> ResendOtpAsync(ResendOtpDto model);
        Task<bool> AssignAdminRoleAsync(string userId);
        Task<List<UserDto>> GetAllUsersAsync();

    }
}
