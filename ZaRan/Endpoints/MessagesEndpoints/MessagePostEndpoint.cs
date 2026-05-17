using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Riok.Mapperly.Abstractions;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Extensions.AuthorizationExtension;
using ZaRan.Services;

namespace ZaRan.Endpoints.MessagesEndpoints;

public class MessagePostEndpoint(AppDbContext dbContext) : Endpoint<MessagePostEndpointRequest, Results<Ok<MessageDetailResponse>, BadRequest>>
{
    public override void Configure()
    {
        Post("/messages");
        Roles(nameof(UserRole.User));
    }

    public override async Task<Results<Ok<MessageDetailResponse>, BadRequest>> ExecuteAsync(MessagePostEndpointRequest req, CancellationToken ct)
    {
        var message = MessagePostEndpointRequest.Map(req);
        var currentUser = await User.GetCurrentUser(dbContext);
        if (currentUser is null)
            return TypedResults.BadRequest();
        message.Sender = currentUser;
        message.SenderId = currentUser.Id;
        var receiver = await dbContext.Users.FindAsync([req.ReceiverId], ct);
        if (receiver is null)
            return TypedResults.BadRequest();
        message.Receiver = receiver;
        message.ReceiverId = req.ReceiverId;
        dbContext.Messages.Add(message);
        await dbContext.SaveChangesAsync(cancellationToken: ct);
        var response = MessageDetailResponse.Map(message);
        return TypedResults.Ok(response);
    }
}

public class MessagePostEndpointRequestValidator : Validator<MessagePostEndpointRequest>
{
    public MessagePostEndpointRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.ReceiverId).NotEmpty();
    }
}

[Mapper]
public partial class MessagePostEndpointRequest
{
    public required string Title { get; set; } = string.Empty;
    public required string Content { get; set; } = string.Empty;
    public required Guid ReceiverId { get; set; }
    public MessageType Type { get; set; } = MessageType.Private;

    [MapperIgnoreTarget(nameof(Message.Id))]
    [MapperIgnoreTarget(nameof(Message.Sender))]
    [MapperIgnoreTarget(nameof(Message.SenderId))]
    [MapperIgnoreTarget(nameof(Message.Receiver))]
    [MapperIgnoreTarget(nameof(Message.CreatedAt))]
    [MapperIgnoreTarget(nameof(Message.IsRead))]
    public static partial Message Map(MessagePostEndpointRequest request);
}
