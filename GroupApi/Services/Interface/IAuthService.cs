using GroupApi.DTOs.Auth;
using GroupApi.Entities.Auth;
using Microsoft.AspNetCore.Mvc;

namespace GroupApi.Services.Interface
{
    public interface IAuthService
    {
        Task<IActionResult> RegisterAsync(RegisterUserDto registerDto);
        Task<IActionResult> VerifyEmailAsync(VerifyOtpDto verifyOtpDto);
        Task<IActionResult> ResendOtpAsync(ForgotPasswordDto resendOtpDto);
        Task<IActionResult> LoginAsync(LoginUserDto loginDto);
        Task<IActionResult> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
        Task<IActionResult> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<IActionResult> VerifyOtpAsync(VerifyOtpDto verifyOtpDto);
        Task<IActionResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<IActionResult> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);
    }
}