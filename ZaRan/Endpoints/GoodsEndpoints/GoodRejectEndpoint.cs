using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Extensions.AuthorizationExtension;
using ZaRan.Services;

namespace ZaRan.Endpoints.GoodsEndpoints;

public class GoodRejectEndpoint(AppDbContext dbContext) : Endpoint<GoodRejectRequest, Results<Ok<GoodDetailResponse>, NotFound, UnauthorizedHttpResult, BadRequest>>
{
    public override void Configure()
    {
        Post("/admin/goods/{id:guid}/reject");
        Roles(nameof(UserRole.Admin));
    }

    public override async Task<Results<Ok<GoodDetailResponse>, NotFound, UnauthorizedHttpResult, BadRequest>> ExecuteAsync(GoodRejectRequest req, CancellationToken ct)
    {
        var goodId = Route<Guid>("id");
        var good = await dbContext.Goods
            .Include(g => g.Publisher)
            .FirstOrDefaultAsync(g => g.Id == goodId, ct);
        
        if (good is null)
        {
            return TypedResults.NotFound();
        }

        // 更新商品状态为已拒绝
        good.Status = GoodStatus.Rejected;
        good.UpdatedAt = DateTimeOffset.Now;

        // 创建审核通知消息
        var currentUser = await User.GetCurrentUser(dbContext);
        if (currentUser is null)
        {
            return TypedResults.BadRequest();
        }

        var message = new Message
        {
            Title = $"商品《{good.Name}》审核不通过",
            Content = req.Message,
            Type = MessageType.Review,
            SenderId = currentUser.Id,
            Sender = currentUser,
            ReceiverId = good.PublisherId,
            Receiver = good.Publisher,
            IsRead = false
        };

        dbContext.Messages.Add(message);
        dbContext.Goods.Update(good);
        await dbContext.SaveChangesAsync(ct);
        
        return TypedResults.Ok(GoodDetailResponse.Map(good));
    }
}

public class GoodRejectRequestValidator : Validator<GoodRejectRequest>
{
    public GoodRejectRequestValidator()
    {
        RuleFor(x => x.Message).NotEmpty().WithMessage("审核意见不能为空");
        RuleFor(x => x.Message).MaximumLength(1000).WithMessage("审核意见不能超过1000字符");
    }
}

public class GoodRejectRequest
{
    public required string Message { get; set; } = string.Empty;
}
