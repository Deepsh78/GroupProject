using System.ComponentModel.DataAnnotations;

namespace GroupApi.DTOs.Orders
{
    public class ProcessClaimCodeDto
    {
        [Required]
        public string Code { get; set; }
    }
}