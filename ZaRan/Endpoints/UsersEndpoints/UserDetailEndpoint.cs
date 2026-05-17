using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Riok.Mapperly.Abstractions;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Abstraction.Models.Dtos;
using ZaRan.Services;

namespace ZaRan.Endpoints.UsersEndpoints;

public class UserDetailEndpoint(AppDbContext dbContext) : Ep.NoReq.Res<Results<Ok<UserDetailResponse>, NotFound>>
{
    
    public override void Configure()
    {
        Get("/users/{userId:guid}");
    }

    public override async Task<Results<Ok<UserDetailResponse>, NotFound>> ExecuteAsync(CancellationToken ct)
    {
        var userId = Route<Guid>("userId");
        var user = await dbContext.Users.FindAsync([userId], cancellationToken: ct);
        if (user is null)
            return TypedResults.NotFound();
        var resp = UserDetailResponse.Map(user);
        if (User.IsInRole(nameof(UserRole.Admin)))
        {
            resp.Email = user.Email;
        }
        return TypedResults.Ok(resp);
    }
}

[Mapper]
public partial class UserDetailResponse : ResponseBase
{
    public required Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Nickname { get; set; }
    public string? Signature { get; set; }

    [MapperIgnore]
    public string? Email { get; set; }
    public UserRole UserRole { get; set; }
    public UserStatus UserStatus { get; set; }
    public DateTimeOffset? UnbanTime { get; set; }
    public bool IsBanned { get; set; }
    public double Balance { get; set; } = 0;
    public required DateTimeOffset CreatedAt { get; set; }
    public required DateTimeOffset LastLoginAt { get; set; }
    public string? Avatar { get; set; }

    [MapperIgnoreSource(nameof(User.Email))]
    public static partial UserDetailResponse Map(User user);
}