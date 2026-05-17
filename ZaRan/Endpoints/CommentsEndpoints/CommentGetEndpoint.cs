using System;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Abstraction.Models.Dtos;
using ZaRan.Endpoints.UsersEndpoints;
using ZaRan.Extensions.AuthorizationExtension;
using ZaRan.Services;

namespace ZaRan.Endpoints.CommentsEndpoints;

public class CommentGetEndpoint(AppDbContext dbContext) : EndpointWithoutRequest<Results<Ok<CommentDetailResponse>, NotFound>>
{
    public override void Configure()
    {
        Get("/comments/{id:guid}");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<CommentDetailResponse>, NotFound>> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var comment = await dbContext.Comments
            .Include(c => c.Sender)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
        if (comment is null)
            return TypedResults.NotFound();
        var response = CommentDetailResponse.Map(comment);
        return TypedResults.Ok(response);
    }
}
