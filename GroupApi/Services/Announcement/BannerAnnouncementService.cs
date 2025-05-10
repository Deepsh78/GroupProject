using GroupApi.CommonDomain;
using GroupApi.DTOs.Announcements;
using GroupApi.Entities;
using GroupApi.GenericClasses;
using GroupApi.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GroupApi.Services.Announcements
{
    public class BannerAnnouncementService : IBannerAnnouncementService
    {
        private readonly IGenericRepository<BannerAnnouncement> _announcementRepo;

        public BannerAnnouncementService(IGenericRepository<BannerAnnouncement> announcementRepo)
        {
            _announcementRepo = announcementRepo;
        }

        public async Task<GenericResponse<BannerAnnouncementReadDto>> CreateAsync(BannerAnnouncementCreateDto dto)
        {
            if (dto.StartDate >= dto.EndDate)
                return new ErrorModel(HttpStatusCode.BadRequest, "Start date must be before end date");

            var announcement = new BannerAnnouncement
            {
                AnnouncementId = Guid.NewGuid(),
                Title = dto.Title,
                Content = dto.Content,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _announcementRepo.AddAsync(announcement);
            await _announcementRepo.SaveChangesAsync();

            return new BannerAnnouncementReadDto
            {
                AnnouncementId = announcement.AnnouncementId,
                Title = announcement.Title,
                Content = announcement.Content,
                StartDate = announcement.StartDate,
                EndDate = announcement.EndDate,
                IsActive = announcement.IsActive,
                CreatedAt = announcement.CreatedAt
            };
        }

        public async Task<GenericResponse<IEnumerable<BannerAnnouncementReadDto>>> GetAllActiveAsync()
        {
            var currentTime = DateTime.UtcNow;
            var announcements = await _announcementRepo.TableNoTracking
                .Where(a => a.IsActive && a.StartDate <= currentTime && a.EndDate >= currentTime)
                .ToListAsync();

            var result = announcements.Select(a => new BannerAnnouncementReadDto
            {
                AnnouncementId = a.AnnouncementId,
                Title = a.Title,
                Content = a.Content,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                IsActive = a.IsActive,
                CreatedAt = a.CreatedAt
            });

            return result.ToList();
        }
    }
}