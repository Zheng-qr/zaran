using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Riok.Mapperly.Abstractions;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Abstraction.Models.Dtos;
using ZaRan.Extensions.AuthorizationExtension;
using ZaRan.Services;

namespace ZaRan.Endpoints.CommentsEndpoints;

public class CommentPostEndpoint(AppDbContext dbContext) : Endpoint<CommentPostEndpointRequest, Results<Ok<CommentDetailResponse>, BadRequest>>
{
    public override void Configure()
    {
        Post("/comments");
        Roles(nameof(UserRole.User));
    }

    public override async Task<Results<Ok<CommentDetailResponse>, BadRequest>> ExecuteAsync(CommentPostEndpointRequest req, CancellationToken ct)
    {
        var comment = CommentPostEndpointRequest.Map(req);
        var currentUser = await User.GetCurrentUser(dbContext);
        if (currentUser is null)
            return TypedResults.BadRequest();
        comment.Sender = currentUser;
        comment.SenderId = currentUser.Id;
        dbContext.Comments.Add(comment);
        await dbContext.SaveChangesAsync(ct);
        var response = CommentDetailResponse.Map(comment);
        return TypedResults.Ok(response);
    }
}

public class CommentPostEndpointRequestValidator : Validator<CommentPostEndpointRequest>
{
    public CommentPostEndpointRequestValidator()
    {
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.TargetId).NotEmpty();
    }
}

[Mapper]
public partial class CommentPostEndpointRequest
{
    public required string Content { get; set; } = string.Empty;
    public required Guid TargetId { get; set; }

    [MapperIgnoreTarget(nameof(Comment.Id))]
    [MapperIgnoreTarget(nameof(Comment.Sender))]
    [MapperIgnoreTarget(nameof(Comment.SenderId))]
    [MapperIgnoreTarget(nameof(Comment.CreatedAt))]
    public static partial Comment Map(CommentPostEndpointRequest request);
}
