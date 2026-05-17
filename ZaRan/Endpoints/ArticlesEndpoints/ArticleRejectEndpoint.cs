using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Extensions.AuthorizationExtension;
using ZaRan.Services;

namespace ZaRan.Endpoints.ArticlesEndpoints;

public class ArticleRejectEndpoint(AppDbContext dbContext) : Endpoint<ArticleRejectRequest, Results<Ok<ArticleDetailResponse>, NotFound, UnauthorizedHttpResult, BadRequest>>
{
    public override void Configure()
    {
        Post("/admin/articles/{id:guid}/reject");
        Roles(nameof(UserRole.Admin));
    }

    public override async Task<Results<Ok<ArticleDetailResponse>, NotFound, UnauthorizedHttpResult, BadRequest>> ExecuteAsync(ArticleRejectRequest req, CancellationToken ct)
    {
        var articleId = Route<Guid>("id");
        var article = await dbContext.Articles
            .Include(a => a.Author)
            .FirstOrDefaultAsync(a => a.Id == articleId, ct);
        
        if (article is null)
        {
            return TypedResults.NotFound();
        }

        // 更新文章状态为已拒绝
        article.Status = ArticleStatus.Rejected;
        article.UpdatedAt = DateTimeOffset.Now;

        // 创建审核通知消息
        var currentUser = await User.GetCurrentUser(dbContext);
        if (currentUser is null)
        {
            return TypedResults.BadRequest();
        }

        var message = new Message
        {
            Title = $"文章《{article.Title}》审核不通过",
            Content = req.Message,
            Type = MessageType.Review,
            SenderId = currentUser.Id,
            Sender = currentUser,
            ReceiverId = article.AuthorId,
            Receiver = article.Author,
            IsRead = false
        };

        dbContext.Messages.Add(message);
        dbContext.Articles.Update(article);
        await dbContext.SaveChangesAsync(ct);
        
        return TypedResults.Ok(ArticleDetailResponse.Map(article));
    }
}

public class ArticleRejectRequestValidator : Validator<ArticleRejectRequest>
{
    public ArticleRejectRequestValidator()
    {
        RuleFor(x => x.Message).NotEmpty().WithMessage("审核意见不能为空");
        RuleFor(x => x.Message).MaximumLength(1000).WithMessage("审核意见不能超过1000字符");
    }
}

public class ArticleRejectRequest
{
    public required string Message { get; set; } = string.Empty;
}
