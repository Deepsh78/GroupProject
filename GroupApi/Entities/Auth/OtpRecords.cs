
using System.ComponentModel.DataAnnotations;

namespace Skillansar.API.Models;

public class TempUserRegistration
{
    [Key]
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Gender { get; set; }
    public string OTP { get; set; }
    public DateTime OTPExpiration { get; set; }
    public DateTime CreatedAt { get; set; }
}