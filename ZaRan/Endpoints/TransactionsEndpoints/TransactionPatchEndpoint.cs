using System;
using FastEndpoints;
using FastEndpoints.Security;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Riok.Mapperly.Abstractions;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Extensions.AuthorizationExtension;
using ZaRan.Services;
using ZaRan.Services.Languages;

namespace ZaRan.Endpoints.TransactionsEndpoints;

public class TransactionPatchEndpoint(
    AppDbContext dbContext,
    ILanguage language)
    : Endpoint<TransactionPatchEndpointRequest,
        Results<Ok<TransactionDetailResponse>, NotFound, UnauthorizedHttpResult, BadRequest<string>>>
{
    public override void Configure()
    {
        Patch("/transactions/{id:guid}");
        Roles(nameof(UserRole.User));
    }

    public override async
        Task<Results<Ok<TransactionDetailResponse>, NotFound, UnauthorizedHttpResult, BadRequest<string>>> ExecuteAsync(
            TransactionPatchEndpointRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var transaction = await dbContext.Transactions
            .Include(x => x.TargetUser)
            .Include(x => x.Good)
            .Include(x => x.User)
            .FirstOrDefaultAsync(t => t.Id == id, ct);
        if (transaction is null)
            return TypedResults.NotFound();

        var userId = User.GetUserId();
        var isAdmin = User.IsInRole(nameof(UserRole.Admin));
        if (transaction.UserId != userId && !isAdmin)
            return TypedResults.Unauthorized();

        if (transaction.Status == TransactionStatus.Completed)
        {
            return TypedResults.BadRequest(language.TransactionPatch_AlreadyDealed);
        }

        if (transaction.Status == TransactionStatus.Cancelled)
        {
            return TypedResults.BadRequest(language.TransactionPatch_AlreadyCanceled);
        }
        
        await using var dbTransaction = await dbContext.Database.BeginTransactionAsync(ct);

        // 此处做交易判断
        if (req.Status == TransactionStatus.Completed)
        {
            // 判断是否此用户能够产生交易通过结果
            if (transaction.CollectId.HasValue)
            {
                // 收藏交易, 需要接收方同意
                if (transaction.TargetUserId != userId && !isAdmin)
                    return TypedResults.Unauthorized();
            }
            else
            {
                // 商品购买, 本人确认
                if (transaction.UserId != userId && !isAdmin)
                    return TypedResults.Unauthorized();
            }

            if (transaction.Good is null)
            {
                return TypedResults.NotFound();
            }

            // 交易确认
            var money = transaction.Price;
            // 发起转账
            transaction.User!.Balance -= money;
            await dbContext.SaveChangesAsync(ct);
            if (transaction.User!.Balance < 0)
            {
                return TypedResults.BadRequest(language.TransactionPatch_NoEnoughMoney);
            }

            if (!transaction.CollectId.HasValue)
            {
                // 检查商品是否富足
                if (transaction.Good.Stoke is <= 0 and not -1)
                {
                    return TypedResults.BadRequest(language.TransactionPatch_NoEnoughGood);
                }
            }

            transaction.TargetUser!.Balance += money;
            if (transaction.TargetUser!.Balance < 0)
            {
                return TypedResults.BadRequest(language.TransactionPatch_NoEnoughMoney);
            }
            await dbContext.SaveChangesAsync(ct);
            if (transaction.Good.Stoke != -1)
                --transaction.Good.Stoke;
            await dbContext.SaveChangesAsync(ct);
            Collect? collect;
            if (!transaction.CollectId.HasValue)
            {
                // 获得商品唯一收藏 ID
                var max = dbContext.Collects.Where(t => t.GoodId == transaction.GoodId).MaxBy(t => t.Index);
                var currentIndex = (max?.Index ?? 0) + 1;
                // add a new collection
                collect = new Collect
                {
                    User = transaction.User,
                    Good = transaction.Good,
                    Transaction = transaction,
                    UserId = transaction.UserId,
                    GoodId = transaction.GoodId,
                    Index = currentIndex,
                    TransactionId = transaction.Id
                };
                dbContext.Collects.Add(collect);
            }
            else
            {
                collect = await dbContext.Collects.FindAsync([transaction.CollectId.Value], ct);
                if (collect is null)
                    return TypedResults.NotFound();
                collect.User = transaction.TargetUser;
                collect.UserId = transaction.TargetUserId;
                dbContext.Collects.Update(collect);
            }

            await dbContext.SaveChangesAsync(ct);
        }

        transaction.Status = req.Status;
        transaction.UpdatedAt = DateTimeOffset.Now;
        await dbContext.SaveChangesAsync(ct);
        await dbTransaction.CommitAsync(ct);
        var response = TransactionDetailResponse.Map(transaction);
        return TypedResults.Ok(response);
    }
}

public class TransactionPatchEndpointRequestValidator : Validator<TransactionPatchEndpointRequest>
{
    public TransactionPatchEndpointRequestValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
    }
}

[Mapper]
public partial class TransactionPatchEndpointRequest
{
    public TransactionStatus Status { get; set; }
}