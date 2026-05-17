using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Services;

namespace ZaRan.Endpoints.CollectionsEndpoints;

public class CollectionDeleteEndpoint(AppDbContext dbContext)
    : EndpointWithoutRequest<Results<NoContent, NotFound, UnauthorizedHttpResult>>
{
    public override void Configure()
    {
        Delete("/collections/{id:guid}");
        Roles(nameof(UserRole.User));
    }

    public override async Task<Results<NoContent, NotFound, UnauthorizedHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        var collectionId = Route<Guid>("id");
        var collection = await dbContext.Collections
            .FirstOrDefaultAsync(c => c.Id == collectionId, cancellationToken: ct);

        if (collection is null)
        {
            return TypedResults.NotFound();
        }

        // Check if user is the creator or admin
        var userId = User.ClaimValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole(nameof(UserRole.Admin));
        if (userId != collection.CreatorId.ToString() && !isAdmin)
        {
            return TypedResults.Unauthorized();
        }

        dbContext.Collections.Remove(collection);
        await dbContext.SaveChangesAsync(cancellationToken: ct);

        return TypedResults.NoContent();
    }
}
