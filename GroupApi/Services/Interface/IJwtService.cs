using GroupApi.Entities.Auth;

namespace GroupApi.Services.Interface
{
    public interface IJwtService
    {
        Task<string> GenerateJwtTokenAsync(ApplicationUser user);
    }
}
