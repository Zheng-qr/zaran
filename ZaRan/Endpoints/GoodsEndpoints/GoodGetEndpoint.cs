using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Riok.Mapperly.Abstractions;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Abstraction.Models.Dtos;
using ZaRan.Endpoints.ArticlesEndpoints;
using ZaRan.Endpoints.CollectionsEndpoints;
using ZaRan.Endpoints.UsersEndpoints;
using ZaRan.Services;

namespace ZaRan.Endpoints.GoodsEndpoints;

public class GoodGetEndpoint(
        AppDbContext dbContext
    ): EndpointWithoutRequest<Results<Ok<GoodDetailResponse>, NotFound, UnauthorizedHttpResult>>
{
    public override void Configure()
    {
        Get("/goods/{id:guid}");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<GoodDetailResponse>, NotFound, UnauthorizedHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        var goodId = Route<Guid>("id");
        var good = await dbContext.Goods
            .Include(good => good.Publisher)
            .FirstOrDefaultAsync(t=>t.Id == goodId, cancellationToken: ct);
        
        if (good is null)
        {
            return TypedResults.NotFound();
        }
        
        var userId = User.ClaimValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole(nameof(UserRole.Admin));
        
        if (userId != good.PublisherId.ToString() && !isAdmin)
        {
            if (good.Status == GoodStatus.Draft)
            {
                return TypedResults.Unauthorized();
            }
        }
        
        var response = GoodDetailResponse.Map(good);
        return TypedResults.Ok(response);
    }
}

[Mapper]
public partial class GoodDetailResponse : ResponseBase
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }
    public int Stoke { get; set; }
    public double DiscountedPrice { get; set; }
    public string? PublisherName { get; set; }
    public GoodStatus Status { get; set; }
    public string? ImageUrl { get; set; }
    public string? CopyrightInfo { get; set; } = string.Empty; // 版权信息字段
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public UserDetailResponse? Publisher { get; set; }
    public CollectionDetailResponse? Collection { get; set; }
    public Guid RelatedArticleId { get; set; }
    public Guid PublisherId { get; set; }
    public Guid CollectionId { get; set; }

    [MapperIgnoreSource(nameof(Good.RelatedArticle))]
    [MapperIgnoreTarget(nameof(User.Email))]
    public static partial GoodDetailResponse Map(Good good);
}

