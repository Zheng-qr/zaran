using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Riok.Mapperly.Abstractions;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Abstraction.Models.Dtos;
using ZaRan.Endpoints.UsersEndpoints;
using ZaRan.Extensions.AuthorizationExtension;
using ZaRan.Services;

namespace ZaRan.Endpoints.MessagesEndpoints;

public class MessageGetEndpoint(
        AppDbContext dbContext
    ) : EndpointWithoutRequest<Results<Ok<MessageDetailResponse>, NotFound, UnauthorizedHttpResult>>
{
    public override void Configure()
    {
        Get("/messages/{id:guid}");
        Roles(nameof(UserRole.User));
    }

    public override async Task<Results<Ok<MessageDetailResponse>, NotFound, UnauthorizedHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        var messageId = Route<Guid>("id");
        var message = await dbContext.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken: ct);
        if (message is null)
            return TypedResults.NotFound();

        var userId = User.GetUserId().ToString();
        var isAdmin = User.IsInRole(nameof(UserRole.Admin));
        if (userId != message.SenderId.ToString() && userId != message.ReceiverId.ToString() && !isAdmin)
            return TypedResults.Unauthorized();

        var response = MessageDetailResponse.Map(message);
        return TypedResults.Ok(response);
    }
}

[Mapper]
public partial class MessageDetailResponse : ResponseBase
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public MessageType Type { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public UserDetailResponse Sender { get; set; } = null!;
    public UserDetailResponse Receiver { get; set; } = null!;

    [MapperIgnoreSource(nameof(Message.SenderId))]
    [MapperIgnoreSource(nameof(Message.ReceiverId))]
    public static partial MessageDetailResponse Map(Message message);
}
