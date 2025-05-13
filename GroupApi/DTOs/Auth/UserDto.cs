using GroupApi.Constants;
using GroupApi.Constraint;

namespace GroupApi.DTOs.Auth
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GenderType Gender { get; set; }
        public RoleType Role { get; set; }
    }
}
