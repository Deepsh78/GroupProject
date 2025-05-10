using GroupApi.CommonDomain;
using GroupApi.DTOs.Announcements;

namespace GroupApi.Services.Interface
{
    public interface IBannerAnnouncementService
    {
        Task<GenericResponse<BannerAnnouncementReadDto>> CreateAsync(BannerAnnouncementCreateDto dto);
        Task<GenericResponse<IEnumerable<BannerAnnouncementReadDto>>> GetAllActiveAsync();
    }
}
