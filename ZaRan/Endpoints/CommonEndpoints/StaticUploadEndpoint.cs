using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using ZaRan.Abstraction.Models.DbModels;

namespace ZaRan.Endpoints.CommonEndpoints;

public class StaticUploadEndpoint : Ep.NoReq.Res<Results<Ok<StaticUploadEndpointResponse>,BadRequest, UnauthorizedHttpResult>>
{
    public override void Configure()
    {
        Post("/static/upload");
        Roles(nameof(UserRole.Authorized));
        AllowFileUploads(dontAutoBindFormData: true);
    }

    public override async Task<Results<Ok<StaticUploadEndpointResponse>, BadRequest, UnauthorizedHttpResult>> ExecuteAsync(
        CancellationToken ct)
    {
        await foreach (var section in FormFileSectionsAsync(ct))
        {
            if (section is null) continue;
            var fileId = Guid.CreateVersion7();
            var fileExtension = Path.GetExtension(section.Section.ContentDisposition);
            await using var fs = File.Create($"static/uploads/{fileId}.{fileExtension}");
            await section.Section.Body.CopyToAsync(fs, ct);
            await fs.FlushAsync(ct);
            return TypedResults.Ok(new StaticUploadEndpointResponse
            {
                FileUrl = $"/static/uploads/{fileId}.{fileExtension}"
            });
        }

        return TypedResults.BadRequest();
    }
}

public class StaticUploadEndpointResponse
{
    public string FileUrl { get; set; } = string.Empty;
}