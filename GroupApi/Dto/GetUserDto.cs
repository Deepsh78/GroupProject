using GroupApi.Constraint;

namespace GroupApi.Dto
{
    public class GetUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; }
        public string RegisterData { get; set; }
        public bool IsActive { get; set; }
        public GenderType Gender { get; set; }
    }
}
