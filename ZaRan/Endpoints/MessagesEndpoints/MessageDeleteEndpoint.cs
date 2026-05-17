using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Extensions.AuthorizationExtension;
using ZaRan.Services;

namespace ZaRan.Endpoints.MessagesEndpoints;

public class MessageDeleteEndpoint(AppDbContext dbContext) : EndpointWithoutRequest<Results<NoContent, NotFound, UnauthorizedHttpResult>>
{
    public override void Configure()
    {
        Delete("/messages/{id:guid}");
        Roles(nameof(UserRole.User));
    }

    public override async Task<Results<NoContent, NotFound, UnauthorizedHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var message = await dbContext.Messages.FindAsync(new object[] { id }, ct);
        if (message is null)
            return TypedResults.NotFound();

        var userId = User.GetUserId();
        var isAdmin = User.IsInRole(nameof(UserRole.Admin));
        if (userId != message.SenderId && userId != message.ReceiverId && !isAdmin)
            return TypedResults.Unauthorized();

        dbContext.Messages.Remove(message);
        await dbContext.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }
}
