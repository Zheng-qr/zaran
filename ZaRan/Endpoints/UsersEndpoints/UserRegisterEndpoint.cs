using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Riok.Mapperly.Abstractions;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Services;
using ZaRan.Services.Languages;

namespace ZaRan.Endpoints.UsersEndpoints;

public class UserRegisterEndpoint(
    AppDbContext dbContext,
    IPasswordHasher<User> passwordHasher,
    ILanguage languageService
)
    : Endpoint<UserRegisterRequest, Results<Ok<UserDetailResponse>, BadRequest<string>>>
{
    public override void Configure()
    {
        Post("/user/register");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<UserDetailResponse>, BadRequest<string>>>
        ExecuteAsync(UserRegisterRequest req,
            CancellationToken ct)
    {
        var dbUser = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == req.Username || u.Email == req.Email,
            cancellationToken: ct);
        if (dbUser is not null)
            return TypedResults.BadRequest(languageService.UserRegister_UserAlreadyExists);
        var user = UserRegisterRequest.Map(req);
        user.CreatedAt = DateTimeOffset.Now;
        user.HashedPassword = passwordHasher.HashPassword(user, req.Password);
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken: ct);
        return TypedResults.Ok(UserDetailResponse.Map(user));
    }
}

public class UserRegisterRequestValidator : Validator<UserRegisterRequest>
{
    public UserRegisterRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
        RuleFor(x => x.Nickname).NotEmpty();
        RuleFor(x => x.Email).EmailAddress();
    }
}

[Mapper]
public partial class UserRegisterRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Nickname { get; set; }
    public required string Email { get; set; }
    
    [MapValue(nameof(User.UserRole), UserRole.User)]
    [MapValue(nameof(User.UserStatus), UserStatus.Unverified)]
    [MapValue(nameof(User.Balance), 0.0)]
    [MapperIgnoreSource(nameof(Password))]
    [MapperIgnoreTarget(nameof(User.Id))]
    [MapperIgnoreTarget(nameof(User.CreatedAt))]
    [MapperIgnoreTarget(nameof(User.LastLoginAt))]
    [MapperIgnoreTarget(nameof(User.Signature))]
    [MapperIgnoreTarget(nameof(User.Avatar))]
    public static partial User Map(UserRegisterRequest request);
}