// GroupApi.Services.Interface/IJwtService.cs
using GroupApi.Entities.Auth;
using Microsoft.AspNetCore.Identity;

namespace GroupApi.Services.Interface
{
    public interface IJwtService
    {
        string GenerateToken(ApplicationUser user);
    }
}