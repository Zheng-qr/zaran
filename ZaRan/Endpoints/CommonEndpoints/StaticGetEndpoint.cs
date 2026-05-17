using FastEndpoints;
using Microsoft.Extensions.FileProviders;

namespace ZaRan.Endpoints.CommonEndpoints;

public class StaticGetEndpoint([FromKeyedServices("static")] IFileProvider fileProvider) : EndpointWithoutRequest
{
    
    public override void Configure()
    {
        Get("/static/{*path}");
        AllowAnonymous();
    }

    public override Task HandleAsync(CancellationToken ct)
    {
        // check if the file exists
        var path = Route<string>("path");
        if (string.IsNullOrWhiteSpace(path))
            return SendNotFoundAsync(ct);
        var fileInfo = fileProvider.GetFileInfo(path);
        if (!fileInfo.Exists || fileInfo.IsDirectory || fileInfo.PhysicalPath is null)
            return SendNotFoundAsync(ct);
        
        return SendFileAsync(new FileInfo(fileInfo.PhysicalPath), cancellation: ct);
    }
}