using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Extensions.AuthorizationExtension;
using ZaRan.Services;

namespace ZaRan.Endpoints.CommentsEndpoints;

public class CommentDeleteEndpoint(AppDbContext dbContext) : EndpointWithoutRequest<Results<NoContent, NotFound, UnauthorizedHttpResult>>
{
    public override void Configure()
    {
        Delete("/comments/{id:guid}");
        Roles(nameof(UserRole.User));
    }

    public override async Task<Results<NoContent, NotFound, UnauthorizedHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var comment = await dbContext.Comments.FindAsync(new object[] { id }, ct);
        if (comment is null)
            return TypedResults.NotFound();

        var userId = User.GetUserId();
        var isAdmin = User.IsInRole(nameof(UserRole.Admin));
        if (userId != comment.SenderId && !isAdmin)
            return TypedResults.Unauthorized();

        dbContext.Comments.Remove(comment);
        await dbContext.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }
}
