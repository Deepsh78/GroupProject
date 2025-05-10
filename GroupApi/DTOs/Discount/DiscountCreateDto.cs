using System.ComponentModel.DataAnnotations;

namespace GroupApi.DTOs.Discount
{
    public class DiscountCreateDto
    {
        [Required]
        public Guid BookId { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal Percentage { get; set; }

        public bool OnSale { get; set; } = false;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
