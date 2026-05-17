using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Extensions.AuthorizationExtension;
using ZaRan.Services;

namespace ZaRan.Endpoints.MessagesEndpoints;

public class MessageReadAllEndpoint(AppDbContext dbContext) : EndpointWithoutRequest<Results<NoContent, UnauthorizedHttpResult>>
{
    public override void Configure()
    {
        Patch("/messages/read-all");
        Roles(nameof(UserRole.User));
    }

    public override async Task<Results<NoContent, UnauthorizedHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        var userId = User.GetUserId();
        
        // 标记当前用户接收的所有未读消息为已读
        var unreadMessages = await dbContext.Messages
            .Where(m => m.ReceiverId == userId && !m.IsRead)
            .ToListAsync(ct);

        foreach (var message in unreadMessages)
        {
            message.IsRead = true;
        }

        await dbContext.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }
}
