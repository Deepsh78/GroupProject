namespace GroupApi.DTOs.Announcements
{
    public class BannerAnnouncementReadDto
    {
        public Guid AnnouncementId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
