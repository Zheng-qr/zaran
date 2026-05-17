using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Services;

namespace ZaRan.Endpoints.CollectionsEndpoints;

public class CollectionPatchEndpoint(AppDbContext dbContext)
    : Endpoint<CollectionPatchEndpointRequest, Results<Ok<CollectionDetailResponse>, NotFound, UnauthorizedHttpResult, BadRequest>>
{
    public override void Configure()
    {
        Patch("/collections/{id:guid}");
        Roles(nameof(UserRole.User));
    }

    public override async Task<Results<Ok<CollectionDetailResponse>, NotFound, UnauthorizedHttpResult, BadRequest>> ExecuteAsync(
        CollectionPatchEndpointRequest req, CancellationToken ct)
    {
        var collectionId = Route<Guid>("id");
        var collection = await dbContext.Collections
            .Include(c => c.Creator)
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

        // Update fields if provided
        if (!string.IsNullOrEmpty(req.Name))
            collection.Name = req.Name;
        
        if (!string.IsNullOrEmpty(req.Summary))
            collection.Summary = req.Summary;
        
        if (!string.IsNullOrEmpty(req.Description))
            collection.Description = req.Description;
        
        if (req.Color != null)
            collection.Color = req.Color;
        
        if (req.Tags != null)
            collection.Tags = req.Tags;
        
        if (req.ChildrenIds != null)
            collection.ChildrenIds = req.ChildrenIds;

        await dbContext.SaveChangesAsync(cancellationToken: ct);
        
        var response = CollectionDetailResponse.Map(collection);
        return TypedResults.Ok(response);
    }
}

public class CollectionPatchEndpointRequestValidator : Validator<CollectionPatchEndpointRequest>
{
    public CollectionPatchEndpointRequestValidator()
    {
        RuleFor(x => x.Name).MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Name));
        RuleFor(x => x.Summary).MaximumLength(500).When(x => !string.IsNullOrEmpty(x.Summary));
        RuleFor(x => x.Description).MaximumLength(2000).When(x => !string.IsNullOrEmpty(x.Description));
    }
}

public class CollectionPatchEndpointRequest
{
    public string? Name { get; set; }
    public string? Summary { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
    public string[]? Tags { get; set; }
    public Guid[]? ChildrenIds { get; set; }
}
