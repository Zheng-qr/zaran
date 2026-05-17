using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using FastEnumUtility;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Services;
using ZaRan.Services.Languages;

namespace ZaRan.Endpoints.RelationshipsEndpoints;

public class UserRelationshipAddEndpoint(
    AppDbContext dbContext,
    ILanguage languageService
) : Ep.NoReq.Res<Results<Created, NoContent, UnauthorizedHttpResult, NotFound, BadRequest<string>>>
{
    public override void Configure()
    {
        Post("/relationship/{userId:guid}/{targetType}/{targetId:guid}");
        Roles(nameof(UserRole.User));
    }

    public override async Task<Results<Created, NoContent, UnauthorizedHttpResult, NotFound, BadRequest<string>>>
        ExecuteAsync(CancellationToken ct)
    {
        var targetId = Route<Guid>("targetId");
        var userId = Route<Guid>("userId");
        var targetType = Route<string>("targetType");

        var currentUser = await dbContext.Users.FindAsync([userId], ct);
        if (currentUser == null)
        {
            return TypedResults.NotFound();
        }

        if (!User.IsInRole(nameof(UserRole.Admin)) && userId.ToString() != User.ClaimValue(ClaimTypes.NameIdentifier))
            return TypedResults.Unauthorized();

        var relationShipType = FastEnum.Parse<RelationShipType>(targetType, ignoreCase: true);
        var userRelationship = await dbContext.RelationShips
            .FirstOrDefaultAsync(x => x.Type == relationShipType && x.ActorId == userId && x.Target == targetId, ct);


        if (userRelationship != null)
        {
            return TypedResults.BadRequest(languageService.UserRelationShipAdd_AlreadyExists);
        }

        var newRelationship = new RelationShip
        {
            Actor = currentUser,
            Target = targetId,
            Type = relationShipType
        };
        dbContext.RelationShips.Add(newRelationship);
        await dbContext.SaveChangesAsync(ct);
        return TypedResults.Created();
    }
}