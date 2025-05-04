using GroupApi.Entities.Auth;

namespace GroupApi.Services.Interface
{
    public interface IJwtService
    {
        string GenerateToken(ApplicationUser user);
        string GenerateRefreshToken();
        bool ValidateRefreshToken(ApplicationUser user, string refreshToken);
    }
}