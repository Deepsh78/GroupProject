// GroupApi.Services.Interface/IEmailService.cs
namespace GroupApi.Services.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}