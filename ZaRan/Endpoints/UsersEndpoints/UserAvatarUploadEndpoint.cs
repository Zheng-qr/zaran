using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.FileProviders;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Services;

namespace ZaRan.Endpoints.UsersEndpoints;

public class UserAvatarUploadEndpoint(
    AppDbContext dbContext
    )
    : Ep.NoReq.Res<Results<Ok<UserAvatarUploadResponse>, UnauthorizedHttpResult, NotFound, NoContent>>
{
    public override void Configure()
    {
        Post("/users/{userId:guid}/avatar");
        AllowFileUploads(dontAutoBindFormData: true);
    }

    public override async Task<Results<Ok<UserAvatarUploadResponse>, UnauthorizedHttpResult, NotFound, NoContent>>
        ExecuteAsync(CancellationToken ct)
    {
        // Check if the user exists
        var userId = Route<Guid>("userId");
        // check if userId is login user
        if (!User.IsInRole(nameof(UserRole.Admin)) || userId.ToString() != User.ClaimValue(ClaimTypes.NameIdentifier))
            return TypedResults.Unauthorized();
        var user = await dbContext.Users.FindAsync([userId], ct);
        if (user is null)
            return TypedResults.NotFound();

        // Check if the user is banned
        if (user.IsBanned)
            return TypedResults.Unauthorized();

        await foreach (var section in FormFileSectionsAsync(ct))
        {
            if (section is null) continue;
            var fileId = Guid.CreateVersion7();
            // get the file extension
            var fileExtension = Path.GetExtension(section.Section.ContentDisposition);
            await using var fs = File.Create($"static/avatars/{fileId}.{fileExtension}");
            await section.Section.Body.CopyToAsync(fs, ct);
            await fs.FlushAsync(ct);

            // Update the user avatar
            user.Avatar = $"/static/avatars/{fileId}.{fileExtension}";
            dbContext.Users.Update(user);
            await dbContext.SaveChangesAsync(cancellationToken: ct);
            return TypedResults.Ok(new UserAvatarUploadResponse
            {
                AvatarUrl = $"/static/avatars/{fileId}.{fileExtension}"
            });
        }

        return TypedResults.NoContent();
    }
}

public class UserAvatarUploadResponse
{
    public required string AvatarUrl { get; set; }
}