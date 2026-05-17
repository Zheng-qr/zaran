using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Services;

namespace ZaRan.Endpoints.UsersEndpoints;

public class UserStatusEndpoint(AppDbContext dbContext) : EndpointWithoutRequest<Results<Ok<UserDetailResponse>, UnauthorizedHttpResult>>
{
    public override void Configure()
    {
        Get("/user/status");
        Roles(nameof(UserRole.User));
    }

    public override async Task<Results<Ok<UserDetailResponse>, UnauthorizedHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        var userId = HttpContext.User.ClaimValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return TypedResults.Unauthorized();
        }

        var user = await dbContext.Users.FindAsync([Guid.Parse(userId)], ct);
        if (user is null)
        {
            return TypedResults.Unauthorized();
        }
        return TypedResults.Ok(UserDetailResponse.Map(user));
    }
}