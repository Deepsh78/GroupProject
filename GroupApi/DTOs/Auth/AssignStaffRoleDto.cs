using System.ComponentModel.DataAnnotations;

namespace GroupApi.DTOs.Auth
{
    public class AssignStaffRoleDto
    {
        [Required]
        public string UserId { get; set; }
    }
}
