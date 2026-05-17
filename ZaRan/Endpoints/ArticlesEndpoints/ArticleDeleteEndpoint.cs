using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Extensions.AuthorizationExtension;
using ZaRan.Services;

namespace ZaRan.Endpoints.ArticlesEndpoints;

public class ArticleDeleteEndpoint(AppDbContext dbContext) : Ep.NoReq.Res<Results<NoContent, NotFound, UnauthorizedHttpResult>>
{
    public override void Configure()
    {
        Delete("/articles/{id:guid}");
        Roles(nameof(UserRole.Publisher));
    }
    
    public override async Task<Results<NoContent, NotFound, UnauthorizedHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        var articleId = Route<Guid>("id");
        var article = await dbContext.Articles
            .Include(a => a.Author)
            .FirstOrDefaultAsync(ar => ar.Id == articleId, ct);
        if (article is null)
        {
            return TypedResults.NotFound();
        }

        if (!User.IsInRole(nameof(UserRole.Admin)) && article.AuthorId != User.GetUserId())
        {
            return TypedResults.Unauthorized();
        }

        dbContext.Articles.Remove(article);
        await dbContext.SaveChangesAsync(ct);
        
        return TypedResults.NoContent();
    }
}