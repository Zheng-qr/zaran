using FastEndpoints;
using FastEnumUtility;
using Microsoft.EntityFrameworkCore;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Abstraction.Models.Dtos;
using ZaRan.Extensions.AuthorizationExtension;
using ZaRan.Services;

namespace ZaRan.Endpoints.ArticlesEndpoints;

public class ArticleListEndpoint(AppDbContext dbContext) : Endpoint<UserTargetSearchRequest, ArrayResult<ArticleDetailResponse>>
{
    public override void Configure()
    {
        Get("/articles/{type}");
        AllowAnonymous();
    }

    public override async Task<ArrayResult<ArticleDetailResponse>> ExecuteAsync(UserTargetSearchRequest req, CancellationToken ct)
    {
        var query = dbContext.Articles.Include(t => t.Author).AsQueryable();
        var reqType = Route<string>("type");
        if (reqType != "all")
        {
            var type = FastEnum.Parse<ArticleType>(reqType, ignoreCase: true);
            query = query.Where(a => a.Type == type);
        }

        if (req.UserId.HasValue)
        {
            query = query.Where(a => a.AuthorId == req.UserId.Value);
        }
        
        if (!User.IsInRole(nameof(UserRole.Admin)) && User.GetUserId() != req.UserId)
        {
            query = query.Where(a => a.Status == ArticleStatus.Published);
        }


        if (!string.IsNullOrEmpty(req.Keyword))
        {
            query = query.Where(a =>
                a.Title.Contains(req.Keyword) ||
                (a.Body != null && a.Body.Contains(req.Keyword)) ||
                (a.Tags.Contains(req.Keyword)) ||
                (a.Summary != null && a.Summary.Contains(req.Keyword))
            );
        }


        var totalCount = await query.CountAsync(ct);
        if (req.Desc)
            query = query.OrderByDescending(t => t.UpdatedAt);
        
        var articles = await query
            .Skip(req.Offset)
            .Take(req.Limit)
            .ToListAsync(ct);

        var result = new ArrayResult<ArticleDetailResponse>
        {
            Total = totalCount,
            Items = articles.Select(ArticleDetailResponse.Map
            ).ToArray()
        };
        return result;
    }
}