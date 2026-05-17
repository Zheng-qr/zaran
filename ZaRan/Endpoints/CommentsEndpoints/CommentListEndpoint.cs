using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Abstraction.Models.Dtos;
using ZaRan.Services;
using ZaRan.Endpoints.UsersEndpoints;

namespace ZaRan.Endpoints.CommentsEndpoints;

public class CommentListEndpoint(AppDbContext dbContext) : Endpoint<CommentListRequest, ArrayResult<CommentDetailResponse>>
{
    public override void Configure()
    {
        Get("/comments/{targetId:guid}");
        AllowAnonymous();
    }

    public override async Task<ArrayResult<CommentDetailResponse>> ExecuteAsync(CommentListRequest req, CancellationToken ct)
    {
        var targetId = Route<Guid>("targetId");
        var query = dbContext.Comments
            .Include(c => c.Sender)
            .Where(c => c.TargetId == targetId);

        var total = await query.CountAsync(ct);
        if (req.Desc)
            query = query.OrderByDescending(c => c.CreatedAt);

        var comments = await query.Skip(req.Offset).Take(req.Limit).ToListAsync(ct);
        return new ArrayResult<CommentDetailResponse>
        {
            Total = total,
            Items = comments.Select(CommentDetailResponse.Map).ToArray()
        };
    }
}

public class CommentListRequest : PaginationRequest
{
}
