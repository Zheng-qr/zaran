using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Extensions.AuthorizationExtension;
using ZaRan.Services;

namespace ZaRan.Endpoints.MessagesEndpoints;

public class MessageReadEndpoint(AppDbContext dbContext) : EndpointWithoutRequest<Results<NoContent, NotFound, UnauthorizedHttpResult>>
{
    public override void Configure()
    {
        Patch("/messages/{id:guid}/read");
        Roles(nameof(UserRole.User));
    }

    public override async Task<Results<NoContent, NotFound, UnauthorizedHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        var messageId = Route<Guid>("id");
        var message = await dbContext.Messages.FindAsync(new object[] { messageId }, ct);
        if (message is null)
            return TypedResults.NotFound();

        var userId = User.GetUserId();
        var isAdmin = User.IsInRole(nameof(UserRole.Admin));
        
        // 只有接收者或管理员可以标记消息为已读
        if (userId != message.ReceiverId && !isAdmin)
            return TypedResults.Unauthorized();

        message.IsRead = true;
        await dbContext.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }
}
