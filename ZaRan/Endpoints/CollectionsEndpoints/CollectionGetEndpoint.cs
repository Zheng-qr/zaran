using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Services;

namespace ZaRan.Endpoints.CollectionsEndpoints;

public class CollectionGetEndpoint(
    AppDbContext dbContext
) : EndpointWithoutRequest<Results<Ok<CollectionDetailResponse>, NotFound, UnauthorizedHttpResult>>
{
    public override void Configure()
    {
        Get("/collections/{id:guid}");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<CollectionDetailResponse>, NotFound, UnauthorizedHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        var collectionId = Route<Guid>("id");
        var collection = await dbContext.Collections
            .Include(c => c.Creator)
            .FirstOrDefaultAsync(c => c.Id == collectionId, cancellationToken: ct);
        
        if (collection is null)
        {
            return TypedResults.NotFound();
        }

        var response = CollectionDetailResponse.Map(collection);
        return TypedResults.Ok(response);
    }
}
