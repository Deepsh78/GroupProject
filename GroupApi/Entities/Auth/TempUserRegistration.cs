using System;
using System.ComponentModel.DataAnnotations;
using GroupApi.Constraint;

namespace GroupApi.Entities.Auth
{
    public class TempUserRegistration
    {
        [Key]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public GenderType Gender { get; set; }
        public string OTP { get; set; }
        public DateTime OTPExpiration { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}