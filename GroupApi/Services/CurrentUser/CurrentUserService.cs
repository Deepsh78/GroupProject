using System.Security.Claims;

namespace GroupApi.Services.CurrentUser
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public Guid UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier); // nameIdentifier is the claim type for the user id
                var validGuid = string.IsNullOrEmpty(userIdClaim?.Value) ?
                    Guid.Empty : Guid.Parse(userIdClaim?.Value!);
                return validGuid;
            }
        }
        public string UserEmail
        {
            get
            {
                var emailClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email);
                return emailClaim?.Value ?? string.Empty;
            }
        }
    }
}
