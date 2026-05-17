using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Abstraction.Models.Dtos;
using ZaRan.Services;
using ZaRan.Extensions.AuthorizationExtension;

namespace ZaRan.Endpoints.TransactionsEndpoints;

public class TransactionListEndpoint(AppDbContext dbContext)
    : Endpoint<SearchRequest, ArrayResult<TransactionDetailResponse>>
{
    public override void Configure()
    {
        Get("/transactions");
        Roles(nameof(UserRole.User));
    }

    public override async Task<ArrayResult<TransactionDetailResponse>> ExecuteAsync(SearchRequest req,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var query = dbContext.Transactions
            .Include(t => t.Good)
            .Include(t=>t.User)
            .Include(t=>t.TargetUser)
            .AsQueryable();

        if (!User.IsInRole(nameof(UserRole.Admin)))
        {
            query = query.Where(t => t.UserId == userId || t.TargetUserId == userId);
        }

        if (!string.IsNullOrEmpty(req.Keyword))
        {
            query = query.Where(t =>
                (t.Good != null && t.Good.Name.Contains(req.Keyword)) ||
                (t.Description != null && t.Description.Contains(req.Keyword)) ||
                (t.TargetUser != null && t.TargetUser.NickName == req.Keyword) ||
                (t.TargetUser != null && t.TargetUser.UserName == req.Keyword)
            );
        }

        if (req.Desc)
            query = query.OrderByDescending(t => t.CreatedAt);

        var total = await query.CountAsync(ct);
        var items = await query.Skip(req.Offset).Take(req.Limit).ToListAsync(ct);

        return new ArrayResult<TransactionDetailResponse>
        {
            Total = total,
            Items = items.Select(TransactionDetailResponse.Map).ToArray()
        };
    }
}