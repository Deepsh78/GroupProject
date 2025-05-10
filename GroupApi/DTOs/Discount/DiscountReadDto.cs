namespace GroupApi.DTOs.Discount
{
    public class DiscountReadDto
    {
        public Guid DiscountId { get; set; }
        public Guid BookId { get; set; }
        public string BookName { get; set; }
        public decimal Percentage { get; set; }
        public bool OnSale { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
