using System.Security.Claims;
using FastEndpoints.Security;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Services;

namespace ZaRan.Extensions.AuthorizationExtension;

public static class UserExtensions
{
    public static async Task<User?> GetCurrentUser(this ClaimsPrincipal user, AppDbContext dbContext)
    {
        var userId = user.ClaimValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return null;

        if (!Guid.TryParse(userId, out var userGuid))
            return null;
        
        var currentUser = await dbContext.Users.FindAsync([userGuid]);
        return currentUser;
    }
    
    // get id
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userId = user.ClaimValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Guid.Empty;

        if (!Guid.TryParse(userId, out var userGuid))
            return Guid.Empty;

        return userGuid;
    }
}