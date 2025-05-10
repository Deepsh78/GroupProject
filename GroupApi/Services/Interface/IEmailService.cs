// GroupApi.Services.Interface/IEmailService.cs
namespace GroupApi.Services.Interface
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string email, string otp, string purpose);
    }
}