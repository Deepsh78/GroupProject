// GroupApi.Entities.Auth/ApplicationUser.cs
using GroupApi.Constraint;
using Microsoft.AspNetCore.Identity;

namespace GroupApi.Entities.Auth
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GenderType Gender { get; set; }
    }
}