using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Riok.Mapperly.Abstractions;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Extensions.AuthorizationExtension;
using ZaRan.Services;

namespace ZaRan.Endpoints.TransactionsEndpoints;

public class TransactionPostEndpoint(AppDbContext dbContext) : Endpoint<TransactionPostEndpointRequest, Results<Ok<TransactionDetailResponse>, BadRequest>>
{
    public override void Configure()
    {
        Post("/transactions");
        Roles(nameof(UserRole.User));
    }

    public override async Task<Results<Ok<TransactionDetailResponse>, BadRequest>> ExecuteAsync(TransactionPostEndpointRequest req, CancellationToken ct)
    {
        var currentUser = await User.GetCurrentUser(dbContext);
        if (currentUser is null)
            return TypedResults.BadRequest();
        var good = await dbContext.Goods
            .Include(t=>t.Publisher)
            .FirstOrDefaultAsync(t => t.Id == req.GoodId, ct);
        if (good is null)
            return TypedResults.BadRequest();
        var transaction = TransactionPostEndpointRequest.Map(req);
        transaction.User = currentUser;
        transaction.UserId = currentUser.Id;
        transaction.Good = good;
        transaction.GoodId = good.Id;
        if (req.CollectId.HasValue)
        {
            // 交易收藏物
            var collect = await dbContext.Collects.FindAsync([req.CollectId], ct);
            if (collect is null || collect.UserId != currentUser.Id)
                return TypedResults.BadRequest();
            var targetUser = await dbContext.Users.FindAsync([transaction.TargetUser], ct);
            if (targetUser is null)
                return TypedResults.BadRequest();
            transaction.TargetUser = targetUser;
            transaction.TargetUserId = targetUser.Id;
            transaction.Price = -Math.Abs(transaction.Price);
        }
        else
        {
            transaction.Price = good.DiscountedPrice;
            transaction.TargetUser = good.Publisher!;
            transaction.TargetUserId = good.PublisherId;
        }
        
        
        transaction.CreatedAt = DateTimeOffset.Now;
        transaction.UpdatedAt = DateTimeOffset.Now;
        dbContext.Transactions.Add(transaction);
        await dbContext.SaveChangesAsync(ct);
        var response = TransactionDetailResponse.Map(transaction);
        return TypedResults.Ok(response);
    }
}

public class TransactionPostEndpointRequestValidator : Validator<TransactionPostEndpointRequest>
{
    public TransactionPostEndpointRequestValidator()
    {
        RuleFor(x => x.GoodId).NotEmpty();
    }
}

[Mapper]
public partial class TransactionPostEndpointRequest
{
    public required Guid GoodId { get; set; }
    public required Guid TargetUserId { get; set; }
    public Guid? CollectId { get; set; }
    public int? Price { get; set; }
    public string? Description { get; set; }

    [MapperIgnoreTarget(nameof(Transaction.Id))]
    [MapperIgnoreTarget(nameof(Transaction.User))]
    [MapperIgnoreTarget(nameof(Transaction.UserId))]
    [MapperIgnoreTarget(nameof(Transaction.Good))]
    [MapperIgnoreTarget(nameof(Transaction.GoodId))]
    [MapperIgnoreTarget(nameof(Transaction.CreatedAt))]
    [MapperIgnoreTarget(nameof(Transaction.UpdatedAt))]
    [MapperIgnoreTarget(nameof(Transaction.Status))]
    [MapperIgnoreTarget(nameof(Transaction.TargetUser))]
    [MapperIgnoreSource(nameof(TransactionPostEndpointRequest.GoodId))]
    public static partial Transaction Map(TransactionPostEndpointRequest request);
}
