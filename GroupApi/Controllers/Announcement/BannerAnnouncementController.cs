using GroupApi.CommonDomain;
using GroupApi.DTOs.Announcements;
using GroupApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GroupApi.Controllers.Announcement
{
    namespace GroupApi.Controllers.Announcements
    {
        [ApiController]
        [Route("api/[controller]")]
        public class BannerAnnouncementController : ControllerBase
        {
            private readonly IBannerAnnouncementService _announcementService;

            public BannerAnnouncementController(IBannerAnnouncementService announcementService)
            {
                _announcementService = announcementService;
            }

            [HttpPost]
            public async Task<IActionResult> Create([FromBody] BannerAnnouncementCreateDto dto)
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ErrorModel(HttpStatusCode.BadRequest, "Invalid input data"));

                var result = await _announcementService.CreateAsync(dto);
                return result.IsSuccess
                    ? CreatedAtAction(nameof(GetAllActive), new { id = result.Data!.AnnouncementId }, result)
                    : StatusCode((int)result.Error!.StatusCode, result);
            }

            [HttpGet("active")]
            public async Task<IActionResult> GetAllActive()
            {
                var result = await _announcementService.GetAllActiveAsync();
                return result.IsSuccess ? Ok(result) : StatusCode((int)result.Error!.StatusCode, result);
            }
        }
    }
}
