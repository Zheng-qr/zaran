using System.Linq;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Abstraction.Models.Dtos;
using ZaRan.Extensions.AuthorizationExtension;
using ZaRan.Services;

namespace ZaRan.Endpoints.MessagesEndpoints;

public class MessageListRequest : SearchRequest
{
    public MessageType? Type { get; set; }
}

public class MessageListEndpoint(AppDbContext dbContext) : Endpoint<MessageListRequest, ArrayResult<MessageDetailResponse>>
{
    public override void Configure()
    {
        Get("/messages");
        Roles(nameof(UserRole.User));
    }

    public override async Task<ArrayResult<MessageDetailResponse>> ExecuteAsync(MessageListRequest req, CancellationToken ct)
    {
        var userId = User.GetUserId();
        var query = dbContext.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Where(m => m.SenderId == userId || m.ReceiverId == userId);

        if (!string.IsNullOrEmpty(req.Keyword))
        {
            query = query.Where(m => m.Title.Contains(req.Keyword) || m.Content.Contains(req.Keyword));
        }

        if (req.Type.HasValue)
        {
            query = query.Where(m => m.Type == req.Type.Value);
        }

        if (req.Desc)
            query = query.OrderByDescending(m => m.CreatedAt);

        var total = await query.CountAsync(ct);
        var messages = await query.Skip(req.Offset).Take(req.Limit).ToListAsync(ct);

        return new ArrayResult<MessageDetailResponse>
        {
            Total = total,
            Items = messages.Select(MessageDetailResponse.Map).ToArray()
        };
    }
}
