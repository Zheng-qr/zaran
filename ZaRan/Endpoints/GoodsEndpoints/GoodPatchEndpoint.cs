using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Riok.Mapperly.Abstractions;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Extensions.AuthorizationExtension;
using ZaRan.Services;

namespace ZaRan.Endpoints.GoodsEndpoints;

public class GoodPatchEndpoint(AppDbContext dbContext) : Endpoint<GoodPatchEndpointRequest, Results<Ok<GoodDetailResponse>, NotFound, UnauthorizedHttpResult, BadRequest>>
{
    public override void Configure()
    {
        Patch("/goods/{id:guid}");
        Roles(nameof(UserRole.Publisher));
    }

    public override async Task<Results<Ok<GoodDetailResponse>, NotFound, UnauthorizedHttpResult, BadRequest>> ExecuteAsync(GoodPatchEndpointRequest req, CancellationToken ct)
    {
        var goodId = Route<Guid>("id");
        var good = await dbContext.Goods
            .Include(g => g.Publisher)
            .FirstOrDefaultAsync(g => g.Id == goodId, cancellationToken: ct);
        
        if (good is null)
        {
            return TypedResults.NotFound();
        }

        // Check if user has permission to edit this good
        var userId = User.ClaimValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole(nameof(UserRole.Admin));
        
        if (userId != good.PublisherId.ToString() && !isAdmin)
        {
            return TypedResults.Unauthorized();
        }


        // 应用更新
        GoodPatchEndpointRequest.UpdateGood(req, good);
        good.UpdatedAt = DateTimeOffset.Now;

        await dbContext.SaveChangesAsync(cancellationToken: ct);
        
        var response = GoodDetailResponse.Map(good);
        return TypedResults.Ok(response);
    }
}

public class GoodPatchEndpointRequestValidator : Validator<GoodPatchEndpointRequest>
{
    public GoodPatchEndpointRequestValidator()
    {
        RuleFor(x => x.Name).MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Name));
        RuleFor(x => x.Price).GreaterThan(0).When(x => x.Price.HasValue);
        RuleFor(x => x.DiscountedPrice).GreaterThanOrEqualTo(0).When(x => x.DiscountedPrice.HasValue);
        RuleFor(x => x.ImageUrl).Matches(@"\/static\/.*").When(x => !string.IsNullOrEmpty(x.ImageUrl));
        RuleFor(x => x.CopyrightInfo).MaximumLength(500).When(x => !string.IsNullOrEmpty(x.CopyrightInfo));
    }
}

[Mapper]
public partial class GoodPatchEndpointRequest
{
    public string? Name { get; set; }
    public double? Price { get; set; }
    public int? Stoke { get; set; }
    public double? DiscountedPrice { get; set; }
    public string? ImageUrl { get; set; }
    public string? PublisherName { get; set; }
    public string? CopyrightInfo { get; set; } // 版权信息字段
    public GoodStatus? Status { get; set; }

    [MapperIgnoreTarget(nameof(Good.CreatedAt))]
    [MapperIgnoreTarget(nameof(Good.Id))]
    [MapperIgnoreTarget(nameof(Good.Publisher))]
    [MapperIgnoreTarget(nameof(Good.RelatedArticle))]
    [MapperIgnoreTarget(nameof(Good.Status))]
    [MapperIgnoreSource(nameof(GoodPatchEndpointRequest.Status))]
    [MapperIgnoreTarget(nameof(Good.UpdatedAt))]
    [MapperIgnoreTarget(nameof(Good.RelatedArticleId))]
    [MapperIgnoreTarget(nameof(Good.PublisherId))]
    public static partial void UpdateGood(GoodPatchEndpointRequest request, Good target);
}
