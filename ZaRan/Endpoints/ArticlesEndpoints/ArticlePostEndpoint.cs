using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Riok.Mapperly.Abstractions;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Extensions.AuthorizationExtension;
using ZaRan.Services;

namespace ZaRan.Endpoints.ArticlesEndpoints;

public class ArticlePostEndpoint(AppDbContext dbContext) : Endpoint<ArticlePostEndpointRequest, Results<Ok<ArticleDetailResponse>, BadRequest>>
{
    public override void Configure()
    {
        Post("/articles");
    }

    public override async Task<Results<Ok<ArticleDetailResponse>, BadRequest>> ExecuteAsync(ArticlePostEndpointRequest req, CancellationToken ct)
    {
        var article = ArticlePostEndpointRequest.Map(req);
        article.Author = (await User.GetCurrentUser(dbContext))!;
        dbContext.Articles.Add(article);
        await dbContext.SaveChangesAsync(cancellationToken: ct);
        var response = ArticleDetailResponse.Map(article);
        return TypedResults.Ok(response);
    }
}

public class ArticlePostEndpointRequestValidator : Validator<ArticlePostEndpointRequest>
{
    public ArticlePostEndpointRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Body).NotEmpty();
        RuleFor(x => x.Tags).NotEmpty();
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.ImageSmallUrl).Matches(@"\/static\/.*");
        RuleFor(x => x.ImageUrl).Matches(@"\/static\/.*");
    }
}

[Mapper]
public partial class ArticlePostEndpointRequest
{
    public required string Title { get; set; } = string.Empty;
    public required string Body { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string[] Tags { get; set; } = [];
    public string? ImageUrl { get; set; } = null;
    public string? Color { get; set; } = null;
    public string? ImageSmallUrl { get; set; } = null;
    public ArticleType Type { get; set; }

    [MapperIgnoreTarget(nameof(Article.Author))]
    [MapperIgnoreTarget(nameof(Article.Id))]
    [MapperIgnoreTarget(nameof(Article.AuthorId))]
    [MapperIgnoreTarget(nameof(Article.CreatedAt))]
    [MapperIgnoreTarget(nameof(Article.UpdatedAt))]
    [MapValue(nameof(Article.Status), ArticleStatus.Draft)]
    public static partial Article Map(ArticlePostEndpointRequest request);
}

