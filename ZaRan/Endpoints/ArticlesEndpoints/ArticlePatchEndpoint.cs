using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Riok.Mapperly.Abstractions;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Extensions.AuthorizationExtension;
using ZaRan.Services;

namespace ZaRan.Endpoints.ArticlesEndpoints;

public class ArticlePatchEndpoint(AppDbContext dbContext) : Endpoint<ArticlePatchEndpointRequest, Results<Ok<ArticleDetailResponse>, NotFound, UnauthorizedHttpResult>>
{
    public override void Configure()
    {
        Patch("/articles/{id:guid}");
    }

    public override async Task<Results<Ok<ArticleDetailResponse>, NotFound, UnauthorizedHttpResult>> ExecuteAsync(ArticlePatchEndpointRequest req, CancellationToken ct)
    {
        var articleId = Route<Guid>("id");
        var article = await dbContext.Articles
            .Include(a=>a.Author)
            .FirstOrDefaultAsync(t=>t.Id == articleId, ct);
        
        if (article is null)
        {
            return TypedResults.NotFound();
        }
        if (!User.IsInRole(nameof(UserRole.Admin)) && article.AuthorId != User.GetUserId())
        {
            return TypedResults.Unauthorized();
        }
        
        if (User.IsInRole(nameof(UserRole.Admin)))
        {
            ArticlePatchEndpointRequest.AdminUpdate(req, article);
        }
        else
        {
            ArticlePatchEndpointRequest.UserUpdate(req, article);
            article.Status = ArticleStatus.Draft;
        }
        
        article.UpdatedAt = DateTimeOffset.Now;
        
        dbContext.Articles.Update(article);
        await dbContext.SaveChangesAsync(ct);
        
        return TypedResults.Ok(ArticleDetailResponse.Map(article));
    }
}

[Mapper]
public partial class ArticlePatchEndpointRequest
{
    public required string Title { get; set; } = string.Empty;
    public required string Body { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string[] Tags { get; set; } = [];
    public string? ImageUrl { get; set; } = null;
    public string? Color { get; set; } = null;
    public string? ImageSmallUrl { get; set; } = null;
    public ArticleType Type { get; set; }
    public ArticleStatus Status { get; set; }
    
    [MapperIgnoreTarget(nameof(Article.Id))]
    [MapperIgnoreTarget(nameof(Article.Author))]
    [MapperIgnoreTarget(nameof(Article.AuthorId))]
    [MapperIgnoreTarget(nameof(Article.CreatedAt))]
    [MapperIgnoreTarget(nameof(Article.UpdatedAt))]
    [MapperIgnoreTarget(nameof(Article.Status))]
    [MapperIgnoreSource(nameof(ArticlePatchEndpointRequest.Status))]
    public static partial void UserUpdate(ArticlePatchEndpointRequest req, Article target);
    
    [MapperIgnoreTarget(nameof(Article.Id))]
    [MapperIgnoreTarget(nameof(Article.Author))]
    [MapperIgnoreTarget(nameof(Article.CreatedAt))]
    [MapperIgnoreTarget(nameof(Article.UpdatedAt))]
    [MapperIgnoreTarget(nameof(Article.Status))]
    [MapperIgnoreSource(nameof(ArticlePatchEndpointRequest.Status))]
    public static partial void AdminUpdate(ArticlePatchEndpointRequest req, Article target);
}