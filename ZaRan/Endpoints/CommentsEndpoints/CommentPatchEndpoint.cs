using System;
using FastEndpoints;
using FastEndpoints.Security;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Riok.Mapperly.Abstractions;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Extensions.AuthorizationExtension;
using ZaRan.Services;

namespace ZaRan.Endpoints.CommentsEndpoints;

public class CommentPatchEndpoint(AppDbContext dbContext) : Endpoint<CommentPatchEndpointRequest, Results<Ok<CommentDetailResponse>, NotFound, UnauthorizedHttpResult, BadRequest>>
{
    public override void Configure()
    {
        Patch("/comments/{id:guid}");
        Roles(nameof(UserRole.User));
    }

    public override async Task<Results<Ok<CommentDetailResponse>, NotFound, UnauthorizedHttpResult, BadRequest>> ExecuteAsync(CommentPatchEndpointRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var comment = await dbContext.Comments
            .Include(c => c.Sender)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
        if (comment is null)
            return TypedResults.NotFound();

        var userId = User.GetUserId();
        var isAdmin = User.IsInRole(nameof(UserRole.Admin));
        if (userId != comment.SenderId && !isAdmin)
            return TypedResults.Unauthorized();

        CommentPatchEndpointRequest.UpdateTarget(req, comment);
        comment.CreatedAt = comment.CreatedAt; // no change
        await dbContext.SaveChangesAsync(ct);
        var response = CommentDetailResponse.Map(comment);
        return TypedResults.Ok(response);
    }
}

public class CommentPatchEndpointRequestValidator : Validator<CommentPatchEndpointRequest>
{
    public CommentPatchEndpointRequestValidator()
    {
        RuleFor(x => x.Content).NotEmpty();
    }
}

[Mapper]
public partial class CommentPatchEndpointRequest
{
    public required string Content { get; set; }

    [MapperIgnoreTarget(nameof(Comment.Id))]
    [MapperIgnoreTarget(nameof(Comment.Sender))]
    [MapperIgnoreTarget(nameof(Comment.SenderId))]
    [MapperIgnoreSource(nameof(Comment.CreatedAt))]
    public static partial void UpdateTarget(CommentPatchEndpointRequest request, Comment target);
}
