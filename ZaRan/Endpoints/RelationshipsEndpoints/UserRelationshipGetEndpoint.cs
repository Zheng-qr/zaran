using System.Linq.Expressions;
using System.Text.Json.Serialization;
using FastEndpoints;
using FastEnumUtility;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Abstraction.Models.Dtos;
using ZaRan.Endpoints.ArticlesEndpoints;
using ZaRan.Endpoints.GoodsEndpoints;
using ZaRan.Endpoints.UsersEndpoints;
using ZaRan.Services;

namespace ZaRan.Endpoints.RelationshipsEndpoints;

public class UserRelationshipGetEndpoint(AppDbContext dbContext)
    : Ep.Req<UserRelationshipGetRequest>.Res<Results<Ok<object>, BadRequest, NotFound>>
{
    public override void Configure()
    {
        Get("/relationship/{typeId}/{userId:guid}");
        Roles(nameof(UserRole.User));
    }


    public override async Task<Results<Ok<object>, BadRequest, NotFound>> ExecuteAsync(
        UserRelationshipGetRequest req,
        CancellationToken ct)
    {
        var userId = Route<Guid>("userId");
        var typeId = Route<string>("typeId");
        IQueryable<RelationShip> userQuery;
        var relationShipType = FastEnum.Parse<RelationShipType>(typeId, ignoreCase: true);
        if (relationShipType == RelationShipType.Unspecified)
        {
            return TypedResults.BadRequest();
        }

        var total = 0;
        if (req.Target)
        {
            userQuery = dbContext.RelationShips.Where(x => x.ActorId == userId);
            total = await userQuery.CountAsync(cancellationToken: ct);
        }
        else
        {
            userQuery = dbContext.RelationShips.Where(x => x.Target == userId);
            total = await userQuery.CountAsync(cancellationToken: ct);
        }

        if (req.Desc)
            userQuery = userQuery.OrderByDescending(t => t.CreatedAt);

        userQuery = userQuery.Skip(req.Offset).Take(req.Limit);

        Expression<Func<RelationShip, Guid>> selector = r => r.ActorId;
        if (req.Target)
        {
            selector = r => r.Target;
        }

        if (total == 0)
        {
            return TypedResults.Ok((object)new { total = 0, items = new object[0] });
        }

        // ReSharper disable CoVariantArrayConversion
        switch (relationShipType)
        {
            case RelationShipType.Follow:
                var rst = userQuery.Join(dbContext.Users,
                        selector,
                        user => user.Id,
                        (relationship, user) => UserDetailResponse.Map(user))
                    .ToArray();
                return TypedResults.Ok((object)new { total = total, items = rst });
            case RelationShipType.ArticleLike:
                var articleRst = userQuery.Join(dbContext.Articles,
                        selector,
                        article => article.Id,
                        (relationship, article) => ArticleDetailResponse.Map(article))
                    .ToArray();

                return TypedResults.Ok((object)new { total = total, items = articleRst });
            case RelationShipType.GoodLike:
                var goodRst = userQuery.Join(dbContext.Goods,
                        selector,
                        good => good.Id,
                        (relationship, good) => GoodDetailResponse.Map(good))
                    .ToArray();
                return TypedResults.Ok((object)new { total = total, items = goodRst });
        }
        // ReSharper restore CoVariantArrayConversion
        return TypedResults.BadRequest();
    }
}

public class UserRelationshipGetRequest : PaginationRequest
{
    [JsonPropertyName("target")] public bool Target { get; set; } = false;
}