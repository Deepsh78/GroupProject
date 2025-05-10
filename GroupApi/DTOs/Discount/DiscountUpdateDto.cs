using System.ComponentModel.DataAnnotations;

namespace GroupApi.DTOs.Discount
{
    public class DiscountUpdateDto
    {
        [Required]
        [Range(0, 100)]
        public decimal Percentage { get; set; }

        public bool OnSale { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
