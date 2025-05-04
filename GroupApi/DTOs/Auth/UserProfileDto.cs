// GroupApi.DTOs.Auth/UserProfileDto.cs
using GroupApi.Constraint;

namespace GroupApi.DTOs.Auth
{
    public class UserProfileDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GenderType Gender { get; set; }
    }
}