using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Services;

namespace ZaRan.Endpoints.GoodsEndpoints;

public class GoodDeleteEndpoint(AppDbContext dbContext) : EndpointWithoutRequest<Results<NoContent, NotFound, UnauthorizedHttpResult>>
{
    public override void Configure()
    {
        Delete("/goods/{id:guid}");
        Roles(nameof(UserRole.Publisher));
    }

    public override async Task<Results<NoContent, NotFound, UnauthorizedHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        var goodId = Route<Guid>("id");
        var good = await dbContext.Goods
            .FirstOrDefaultAsync(g => g.Id == goodId, cancellationToken: ct);
        
        if (good is null)
        {
            return TypedResults.NotFound();
        }

        // Check if user has permission to delete this good
        var userId = User.ClaimValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole(nameof(UserRole.Admin));
        
        if (userId != good.PublisherId.ToString() && !isAdmin)
        {
            return TypedResults.Unauthorized();
        }

        dbContext.Goods.Remove(good);
        await dbContext.SaveChangesAsync(cancellationToken: ct);
        
        return TypedResults.NoContent();
    }
}
