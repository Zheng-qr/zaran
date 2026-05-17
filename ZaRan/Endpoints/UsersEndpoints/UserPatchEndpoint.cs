using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Riok.Mapperly.Abstractions;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Extensions.AuthorizationExtension;
using ZaRan.Services;

namespace ZaRan.Endpoints.UsersEndpoints;

public class UserPatchEndpoint(AppDbContext dbContext, IPasswordHasher<User> passwordHasher) : Endpoint<UserPatchRequest, Results<Ok<UserDetailResponse>, NotFound, UnauthorizedHttpResult>>
{
    public override void Configure()
    {
        Patch("/users/{userId:guid}");
    }

    public override async Task<Results<Ok<UserDetailResponse>, NotFound, UnauthorizedHttpResult>> ExecuteAsync(UserPatchRequest req, CancellationToken ct)
    {
        var userId = Route<Guid>("userId");
        if (!User.IsInRole(nameof(UserRole.Admin)) && User.GetUserId() != userId)
        {
            return TypedResults.Unauthorized();
        }
        
        var dbUser = await dbContext.Users.FindAsync([userId], cancellationToken: ct);
        
        if (dbUser is null)
            return TypedResults.NotFound();
        
        UserPatchRequest.UserApplyUpdate(req, dbUser);
        
        if (User.IsInRole(nameof(UserRole.Admin)))
        {
            UserPatchRequest.AdminApplyUpdate(req, dbUser);
        }
        
        if (req.Password is not null)
            dbUser.HashedPassword = passwordHasher.HashPassword(dbUser, req.Password);
        
        dbContext.Users.Update(dbUser);
        await dbContext.SaveChangesAsync(cancellationToken: ct);
        var resp = UserDetailResponse.Map(dbUser);
        return TypedResults.Ok(resp);
    }
}

[Mapper(AllowNullPropertyAssignment = false)]
public partial class UserPatchRequest
{
    public string? NickName { get; set; }
    public string? Email { get; set; }

    [MapperIgnore]
    public string? Password { get; set; }
    public string? Avatar { get; set; }
    public string? Signature { get; set; }
    public double? Balance { get; set; }
    public UserRole? UserRole { get; set; }
    public UserStatus? UserStatus { get; set; }
    public DateTimeOffset? UnbanTime { get; set; }

    [MapperIgnoreTarget(nameof(User.Id))]
    [MapperIgnoreTarget(nameof(User.UserName))]
    [MapperIgnoreTarget(nameof(User.Balance))]
    [MapperIgnoreSource(nameof(User.Balance))]
    [MapperIgnoreSource(nameof(User.UserRole))]
    [MapperIgnoreTarget(nameof(UserRole))]
    [MapperIgnoreTarget(nameof(User.UserStatus))]
    [MapperIgnoreSource(nameof(UserStatus))]
    [MapperIgnoreTarget(nameof(User.UnbanTime))]
    [MapperIgnoreSource(nameof(UnbanTime))]
    [MapperIgnoreTarget(nameof(User.CreatedAt))]
    [MapperIgnoreTarget(nameof(User.LastLoginAt))]
    public static partial void UserApplyUpdate(UserPatchRequest request, User target);
    
    [MapperIgnoreTarget(nameof(User.Id))]
    [MapperIgnoreTarget(nameof(User.UserName))]
    [MapperIgnoreTarget(nameof(User.CreatedAt))]
    [MapperIgnoreTarget(nameof(User.LastLoginAt))]
    public static partial void AdminApplyUpdate(UserPatchRequest request, User target);

}