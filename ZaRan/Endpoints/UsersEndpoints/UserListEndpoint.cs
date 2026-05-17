using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Abstraction.Models.Dtos;
using ZaRan.Services;

namespace ZaRan.Endpoints.UsersEndpoints;

public class UserListEndpoint(AppDbContext dbContext) : Endpoint<SearchRequest, Ok<ArrayResult<UserDetailResponse>>>
{
    public override void Configure()
    {
        Get("/users");
        Roles(nameof(UserRole.User));
    }

    public override async Task<Ok<ArrayResult<UserDetailResponse>>> ExecuteAsync(SearchRequest req, CancellationToken ct)
    {
        var users = dbContext.Users.AsQueryable();
        if (req.Keyword is not null)
        {
            users = users.AsNoTracking().Where(u =>
                u.Id.ToString().Contains(req.Keyword) ||
                u.UserName.Contains(req.Keyword) ||
                u.Email.Contains(req.Keyword) ||
                u.NickName.Contains(req.Keyword));
        }
        var count = await users.CountAsync(ct);
        var res = await users.Skip(req.Offset).Take(req.Limit).Select(u => UserDetailResponse.Map(u)).ToArrayAsync(cancellationToken: ct);
                                                                          return TypedResults.Ok(new ArrayResult<UserDetailResponse>
        {
            Total = count,
            Items = res
        });
    }
}

