using GroupApi.Constraint;
using GroupApi.Constants;

namespace GroupApi.Entities.Auth
{
    public class PendingUser
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PasswordHash { get; set; }
        public GenderType Gender { get; set; }
        public RoleType Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}