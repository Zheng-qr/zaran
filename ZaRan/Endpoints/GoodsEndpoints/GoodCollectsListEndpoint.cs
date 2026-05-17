using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Riok.Mapperly.Abstractions;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Abstraction.Models.Dtos;
using ZaRan.Endpoints.UsersEndpoints;
using ZaRan.Services;

namespace ZaRan.Endpoints.GoodsEndpoints;

public class GoodCollectsListEndpoint(AppDbContext dbContext) : Endpoint<PaginationRequest,
    Results<Ok<ArrayResult<GoodCollectsListEndpointResponse>>, NotFound>>
{
    public override void Configure()
    {
        Get("/goods/{goodId:guid}/collects");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<ArrayResult<GoodCollectsListEndpointResponse>>, NotFound>> ExecuteAsync(
        PaginationRequest req, CancellationToken ct)
    {
        // get good
        var goodId = Route<Guid>("goodId");
        var good = await dbContext.Goods.FindAsync([goodId], ct);
        if (good == null)
            return TypedResults.NotFound();

        var collects = dbContext.Collects.Include(t => t.User)
            .Where(t => t.GoodId == goodId)
            .AsQueryable();

        if (req.Desc)
        {
            collects = collects.OrderByDescending(t => t.Index);
        }
        else
        {
            collects = collects.OrderBy(t => t.Index);
        }

        var total = await collects.CountAsync(ct);
        collects = collects.Skip(req.Offset).Take(req.Limit);

        var res = await collects.ToArrayAsync(cancellationToken: ct);
        return TypedResults.Ok(
            new ArrayResult<GoodCollectsListEndpointResponse>(
                res.Select(GoodCollectsListEndpointResponse.Map).ToArray(), total));
    }
}

[Mapper]
public partial class GoodCollectsListEndpointResponse : ResponseBase
{
    public Guid Id { get; set; }
    public UserDetailResponse? User { get; set; }
    public int Index { get; set; }

    [MapperIgnoreSource(nameof(Collect.Transaction))]
    [MapperIgnoreSource(nameof(Collect.TransactionId))]
    [MapperIgnoreSource(nameof(Collect.Good))]
    [MapperIgnoreSource(nameof(Collect.GoodId))]
    [MapperIgnoreSource(nameof(Collect.UserId))]
    public static partial GoodCollectsListEndpointResponse Map(Collect collect);
}