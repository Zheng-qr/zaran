using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Riok.Mapperly.Abstractions;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Extensions.AuthorizationExtension;
using ZaRan.Services;

namespace ZaRan.Endpoints.CollectionsEndpoints;

public class CollectionPostEndpoint(AppDbContext dbContext) 
    : Endpoint<CollectionPostEndpointRequest, Results<Ok<CollectionDetailResponse>, BadRequest>>
{
    public override void Configure()
    {
        Post("/collections");
        Roles(nameof(UserRole.User));
    }

    public override async Task<Results<Ok<CollectionDetailResponse>, BadRequest>> ExecuteAsync(
        CollectionPostEndpointRequest req, CancellationToken ct)
    {
        var collection = CollectionPostEndpointRequest.Map(req);
        collection.Creator = (await User.GetCurrentUser(dbContext))!;
        
        dbContext.Collections.Add(collection);
        await dbContext.SaveChangesAsync(cancellationToken: ct);
        
        var response = CollectionDetailResponse.Map(collection);
        return TypedResults.Ok(response);
    }
}

public class CollectionPostEndpointRequestValidator : Validator<CollectionPostEndpointRequest>
{
    public CollectionPostEndpointRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Summary).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.ChildrenIds).NotNull();
    }
}

[Mapper]
public partial class CollectionPostEndpointRequest
{
    public required string Name { get; set; } = string.Empty;
    public required string Summary { get; set; } = string.Empty;
    public required string Description { get; set; } = string.Empty;
    public string? Color { get; set; }
    public string[]? Tags { get; set; }
    public CollectionType Type { get; set; }
    public Guid[] ChildrenIds { get; set; } = [];

    [MapperIgnoreTarget(nameof(Collection.Creator))]
    [MapperIgnoreTarget(nameof(Collection.Id))]
    [MapperIgnoreTarget(nameof(Collection.CreatorId))]
    public static partial Collection Map(CollectionPostEndpointRequest request);
}
