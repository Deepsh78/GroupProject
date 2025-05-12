using GroupApi.Services.Interface;
using System.Net.Mail;

namespace GroupApi.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendOtpEmailAsync(string email, string otp, string purpose)
        {
            string subject = purpose == "registration" ? "Verify Your Account" : "Password Reset OTP";
            string body = $"Your OTP for {purpose} is: {otp}. It will expire in 10 minutes.";
            await SendEmailAsync(email, subject, body);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
            {
                Port = int.Parse(_configuration["EmailSettings:SmtpPort"]),
                Credentials = new System.Net.NetworkCredential(
                    _configuration["EmailSettings:Username"],
                    _configuration["EmailSettings:Password"]),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:SenderEmail"], _configuration["EmailSettings:SenderName"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
        public async Task SendClaimCodeWithBillAsync(string email, string claimCode, decimal totalAmount, Guid orderId)
        {
            string subject = "Your Bookstore Order & Claim Code";
            string body = $@"
        Dear Customer,

        Thank you for your purchase!

        Order ID: {orderId}
        Total Amount: ${totalAmount}
        Claim Code: {claimCode}

        Please keep this code safe — you'll need it to claim your order.

        Regards,
        Bookstore Team
    ";

            await SendEmailAsync(email, subject, body);
        }

    }
}
