using System.ComponentModel.DataAnnotations;

namespace GroupApi.DTOs.Orders
{
    public class ProcessClaimCodeDto
    {
        [Required]
        public string ClaimCode { get; set; }

        [Required]
        public Guid OrderId { get; set; }
    }
}