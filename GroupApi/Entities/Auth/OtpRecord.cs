// GroupApi.Entities.Auth/OtpRecord.cs
namespace GroupApi.Entities.Auth
{
    public class OtpRecord
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Code { get; set; }
        public DateTime Expiry { get; set; }
        public bool IsForPasswordReset { get; set; }
    }
}