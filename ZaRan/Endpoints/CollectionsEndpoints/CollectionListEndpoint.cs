using FastEndpoints;
using FastEnumUtility;
using Microsoft.EntityFrameworkCore;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Abstraction.Models.Dtos;
using ZaRan.Services;

namespace ZaRan.Endpoints.CollectionsEndpoints;

public class CollectionListEndpoint(AppDbContext dbContext) : Endpoint<CollectionListRequest, ArrayResult<CollectionDetailResponse>>
{
    public override void Configure()
    {
        Get("/collections");
        AllowAnonymous();
    }

    public override async Task<ArrayResult<CollectionDetailResponse>> ExecuteAsync(CollectionListRequest req, CancellationToken ct)
    {
        var query = dbContext.Collections.Include(c => c.Creator).AsQueryable();

        // Filter by type if specified
        if (!string.IsNullOrEmpty(req.Type))
        {
            var type = FastEnum.Parse<CollectionType>(req.Type, ignoreCase: true);
            query = query.Where(c => c.Type == type);
        }

        // Filter by creator if specified
        if (req.CreatorId.HasValue)
        {
            query = query.Where(c => c.CreatorId == req.CreatorId.Value);
        }

        // Search functionality
        if (!string.IsNullOrEmpty(req.Keyword))
        {
            query = query.Where(c =>
                c.Name.Contains(req.Keyword) ||
                c.Summary.Contains(req.Keyword) ||
                c.Description.Contains(req.Keyword) ||
                (c.Tags != null && c.Tags.Contains(req.Keyword))
            );
        }

        var totalCount = await query.CountAsync(ct);
        
        if (req.Desc)
            query = query.OrderByDescending(c => c.Id);
        else
            query = query.OrderBy(c => c.Id);

        var collections = await query
            .Skip(req.Offset)
            .Take(req.Limit)
            .ToListAsync(ct);

        var result = new ArrayResult<CollectionDetailResponse>
        {
            Total = totalCount,
            Items = collections.Select(CollectionDetailResponse.Map).ToArray()
        };
        
        return result;
    }
}

public class CollectionListRequest : SearchRequest
{
    public string? Type { get; set; }
    public Guid? CreatorId { get; set; }
}
