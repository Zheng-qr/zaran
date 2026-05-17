using System.Runtime.CompilerServices;
using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Riok.Mapperly.Abstractions;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Abstraction.Models.Dtos;
using ZaRan.Endpoints.UsersEndpoints;
using ZaRan.Services;

namespace ZaRan.Endpoints.ArticlesEndpoints;

public class ArticleGetEndpoint(
        AppDbContext dbContext
    ): EndpointWithoutRequest<Results<Ok<ArticleDetailResponse>, NotFound, UnauthorizedHttpResult>>
{
    public override void Configure()
    {
        Get("/articles/{id:guid}");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<ArticleDetailResponse>, NotFound, UnauthorizedHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        var articleId = Route<Guid>("id");
        var article = await dbContext.Articles
            .Include(article => article.Author)
            .FirstOrDefaultAsync(t=>t.Id == articleId, cancellationToken: ct);
        if (article is null)
        {
            return TypedResults.NotFound();
        }

        if (article.Status != ArticleStatus.Published)
        {
            // check if is the author
            var userId = User.ClaimValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole(nameof(UserRole.Admin));
            if (userId != article.AuthorId.ToString() && !isAdmin)
            {
                return TypedResults.Unauthorized();
            }
        }
        
        var response = ArticleDetailResponse.Map(article);
        return TypedResults.Ok(response);
    }
}

[Mapper]
public partial class ArticleDetailResponse : ResponseBase
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public ArticleStatus Status { get; set; }
    public ArticleType Type { get; set; }
    public string? Body { get; set; }
    public string? Summary { get; set; }
    public string[] Tags { get; set; } = [];
    public string? ImageUrl { get; set; } = null;
    public string? ImageSmallUrl { get; set; } = null;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public UserDetailResponse? Author { get; set; }
    public string? Color { get; set; } = null;
    
    public static partial ArticleDetailResponse Map(Article article);
}