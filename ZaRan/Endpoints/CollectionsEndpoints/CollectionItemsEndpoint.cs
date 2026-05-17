using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Abstraction.Models.Dtos;
using ZaRan.Endpoints.ArticlesEndpoints;
using ZaRan.Endpoints.GoodsEndpoints;
using ZaRan.Endpoints.UsersEndpoints;
using ZaRan.Services;

namespace ZaRan.Endpoints.CollectionsEndpoints;

public class CollectionItemsEndpoint(AppDbContext dbContext)
    : Ep.Req<PaginationRequest>.Res< Results<Ok<object>, NotFound, BadRequest>>
{
    public override void Configure()
    {
        Get("/collections/{id:guid}/items");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<object>, NotFound, BadRequest>> ExecuteAsync(PaginationRequest req, CancellationToken ct)
    {
        var collectionId = Route<Guid>("id");
        var collection = await dbContext.Collections
            .FirstOrDefaultAsync(c => c.Id == collectionId, cancellationToken: ct);

        if (collection is null)
        {
            return TypedResults.NotFound();
        }

        if (collection.ChildrenIds == null || collection.ChildrenIds.Length == 0)
        {
            return TypedResults.Ok((object)new { total = 0, items = new object[0] });
        }

        var childrenIds = collection.ChildrenIds;
        var totalCount = childrenIds.Length;

        // Apply pagination to the IDs first
        var paginatedIds = childrenIds
            .Skip(req.Offset)
            .Take(req.Limit)
            .ToArray();

        if (paginatedIds.Length == 0)
        {
            return TypedResults.Ok((object)new { total = totalCount, items = new object[0] });
        }
        
        switch (collection.Type)
        {
            case CollectionType.Article:
            case CollectionType.Story:
                var articles = await dbContext.Articles
                    .Include(a => a.Author)
                    .Where(a => paginatedIds.Contains(a.Id))
                    .ToListAsync(ct);

                // Maintain the order from ChildrenIds
                var orderedArticles = paginatedIds
                    .Select(id => articles.FirstOrDefault(a => a.Id == id))
                    .Where(a => a != null)
                    .Select(a => ArticleDetailResponse.Map(a!))
                    .ToArray();

                return TypedResults.Ok((object)new { total = totalCount, items = orderedArticles });

            case CollectionType.Good:
                var goods = await dbContext.Goods
                    .Include(g => g.Publisher)
                    .Where(g => paginatedIds.Contains(g.Id))
                    .ToListAsync(ct);

                // Maintain the order from ChildrenIds
                var orderedGoods = paginatedIds
                    .Select(id => goods.FirstOrDefault(g => g.Id == id))
                    .Where(g => g != null)
                    .Select(g => GoodDetailResponse.Map(g!))
                    .ToArray();

                return TypedResults.Ok((object)new { total = totalCount, items = orderedGoods });

            case CollectionType.User:
                var users = await dbContext.Users
                    .Where(u => paginatedIds.Contains(u.Id))
                    .ToListAsync(ct);

                // Maintain the order from ChildrenIds
                var orderedUsers = paginatedIds
                    .Select(id => users.FirstOrDefault(u => u.Id == id))
                    .Where(u => u != null)
                    .Select(u => UserDetailResponse.Map(u!))
                    .ToArray();

                return TypedResults.Ok((object)new { total = totalCount, items = orderedUsers });

            default:
                return TypedResults.BadRequest();
        }
    }
}