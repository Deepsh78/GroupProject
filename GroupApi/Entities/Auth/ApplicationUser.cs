using GroupApi.Constants;
using GroupApi.Constraint;
using Microsoft.AspNetCore.Identity;

namespace GroupApi.Entities.Auth
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GenderType Gender { get; set; }
        public RoleType Role { get; set; } = RoleType.User; 
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
    }
}