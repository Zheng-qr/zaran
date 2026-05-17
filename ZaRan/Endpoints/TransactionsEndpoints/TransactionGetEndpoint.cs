using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Riok.Mapperly.Abstractions;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Abstraction.Models.Dtos;
using ZaRan.Endpoints.GoodsEndpoints;
using ZaRan.Endpoints.UsersEndpoints;
using ZaRan.Extensions.AuthorizationExtension;
using ZaRan.Services;

namespace ZaRan.Endpoints.TransactionsEndpoints;

public class TransactionGetEndpoint(AppDbContext dbContext) : EndpointWithoutRequest<Results<Ok<TransactionDetailResponse>, NotFound, UnauthorizedHttpResult>>
{
    public override void Configure()
    {
        Get("/transactions/{id:guid}");
        Roles(nameof(UserRole.User));
    }

    public override async Task<Results<Ok<TransactionDetailResponse>, NotFound, UnauthorizedHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        var transactionId = Route<Guid>("id");
        var transaction = await dbContext.Transactions
            .Include(t => t.Good)
            .Include(t => t.User)
            .Include(t => t.TargetUser)
            .FirstOrDefaultAsync(t => t.Id == transactionId, ct);
        if (transaction is null)
            return TypedResults.NotFound();

        var userId = User.GetUserId();
        var isAdmin = User.IsInRole(nameof(UserRole.Admin));
        if (transaction.UserId != userId && transaction.TargetUserId != userId && !isAdmin)
            return TypedResults.Unauthorized();

        var response = TransactionDetailResponse.Map(transaction);
        return TypedResults.Ok(response);
    }
}

[Mapper]
public partial class TransactionDetailResponse : ResponseBase
{
    public Guid Id { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public string? Description { get; set; }
    public TransactionStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public UserDetailResponse? User { get; set; }
    public UserDetailResponse? TargetUser { get; set; }
    public GoodDetailResponse? Good { get; set; }

    [MapperIgnoreSource(nameof(Transaction.UserId))]
    [MapperIgnoreSource(nameof(Transaction.TargetUserId))]
    [MapperIgnoreSource(nameof(Transaction.GoodId))]
    public static partial TransactionDetailResponse Map(Transaction transaction);
}
