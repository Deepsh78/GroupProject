using System.ComponentModel.DataAnnotations;

namespace GroupApi.Entities.Auth
{
    public class TempPasswordReset
    {
        [Key]
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string OTP { get; set; }
        public DateTime OTPExpiration { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
