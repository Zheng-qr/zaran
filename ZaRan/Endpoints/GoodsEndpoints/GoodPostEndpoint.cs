using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Riok.Mapperly.Abstractions;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Extensions.AuthorizationExtension;
using ZaRan.Services;

namespace ZaRan.Endpoints.GoodsEndpoints;

public class GoodPostEndpoint(AppDbContext dbContext) : Endpoint<GoodPostEndpointRequest, Results<Ok<GoodDetailResponse>, BadRequest>>
{
    public override void Configure()
    {
        Post("/goods");
        Roles(nameof(UserRole.Publisher));
    }

    public override async Task<Results<Ok<GoodDetailResponse>, BadRequest>> ExecuteAsync(GoodPostEndpointRequest req, CancellationToken ct)
    {
        var good = GoodPostEndpointRequest.Map(req);
        good.Publisher = (await User.GetCurrentUser(dbContext))!;
        good.PublisherName = good.Publisher.UserName;
        good.CreatedAt = DateTimeOffset.Now;
        dbContext.Goods.Add(good);
        await dbContext.SaveChangesAsync(cancellationToken: ct);
        
        var response = GoodDetailResponse.Map(good);
        return TypedResults.Ok(response);
    }
}

public class GoodPostEndpointRequestValidator : Validator<GoodPostEndpointRequest>
{
    public GoodPostEndpointRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.DiscountedPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ImageUrl).Matches(@"\/static\/.*").When(x => !string.IsNullOrEmpty(x.ImageUrl));
        RuleFor(x => x.CopyrightInfo).MaximumLength(500).When(x => !string.IsNullOrEmpty(x.CopyrightInfo));
    }
}

[Mapper]
public partial class GoodPostEndpointRequest
{
    public required string Name { get; set; } = string.Empty;
    public double Price { get; set; }
    public int Stoke { get; set; } = -1;
    public double DiscountedPrice { get; set; }
    public string? PublisherName { get; set; }
    public string? ImageUrl { get; set; }
    public string? CopyrightInfo { get; set; } = string.Empty; // 版权信息字段
    public Guid RelatedArticleId { get; set; }

    [MapperIgnoreTarget(nameof(Good.Publisher))]
    [MapperIgnoreTarget(nameof(Good.PublisherId))]
    [MapperIgnoreTarget(nameof(Good.RelatedArticle))]
    [MapperIgnoreTarget(nameof(Good.Id))]
    [MapperIgnoreTarget(nameof(Good.CreatedAt))]
    [MapperIgnoreTarget(nameof(Good.UpdatedAt))]
    [MapperIgnoreTarget(nameof(Good.Status))]
    public static partial Good Map(GoodPostEndpointRequest request);
}
