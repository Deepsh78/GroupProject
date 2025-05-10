namespace GroupApi.DTOs.Orders
{
    public class ClaimCodeDto
    {
        public Guid ClaimCodeId { get; set; }
        public string Code { get; set; }
        public Guid OrderId { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UsedAt { get; set; }
    }
}