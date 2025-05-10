

using System.ComponentModel.DataAnnotations;


namespace GroupApi.DTOs.Announcements
{
    public class BannerAnnouncementCreateDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(1000)]
        public string Content { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }

}
