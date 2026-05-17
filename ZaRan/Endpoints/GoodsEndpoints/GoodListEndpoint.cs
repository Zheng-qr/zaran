using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Abstraction.Models.Dtos;
using ZaRan.Extensions.AuthorizationExtension;
using ZaRan.Services;

namespace ZaRan.Endpoints.GoodsEndpoints;

public class GoodListEndpoint(AppDbContext dbContext) : Endpoint<UserTargetSearchRequest, ArrayResult<GoodDetailResponse>>
{
    public override void Configure()
    {
        Get("/goods");
        AllowAnonymous();
    }

    public override async Task<ArrayResult<GoodDetailResponse>> ExecuteAsync(UserTargetSearchRequest req, CancellationToken ct)
    {
        var query = dbContext.Goods
            .Include(g => g.Publisher)
            .AsQueryable();

        // Filter by user if provided
        if (req.UserId.HasValue)
        {
            query = query.Where(g => g.PublisherId == req.UserId.Value);
        }

        // Filter by status based on user permissions
        if (!User.IsInRole(nameof(UserRole.Admin)) && User.GetUserId() != req.UserId)
        {
            query = query.Where(g => g.Status == GoodStatus.Available);
        }

        // Filter by keyword if provided
        if (!string.IsNullOrEmpty(req.Keyword))
        {
            query = query.Where(g =>
                g.Name.Contains(req.Keyword) ||
                (g.PublisherName != null && g.PublisherName.Contains(req.Keyword))
            );
        }

        if (req.Desc)
            query = query.OrderByDescending(g => g.CreatedAt);

        var totalCount = await query.CountAsync(ct);
        var goods = await query
            .Skip(req.Offset)
            .Take(req.Limit)
            .ToListAsync(ct);
        
        var result = new ArrayResult<GoodDetailResponse>
        {
            Total = totalCount,
            Items = goods.Select(GoodDetailResponse.Map).ToArray()
        };
        return result;
    }
}
