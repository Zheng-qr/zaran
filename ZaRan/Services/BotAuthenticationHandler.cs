using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using ZaRan.Abstraction.Models.DbModels;

namespace ZaRan.Services;

public class BotAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,IConfiguration configuration) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    internal const string SchemeName = "ApiKey";
    internal const string HeaderName = "x-api-key";
    readonly string _apiKey = configuration["Auth:ApiKey"] ?? throw new InvalidOperationException("Api key not set in appsettings.json");
    
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Request.Headers.TryGetValue(HeaderName, out var extractedApiKey);

        if (!IsPublicEndpoint() && !extractedApiKey.Equals(_apiKey))
            return Task.FromResult(AuthenticateResult.NoResult());

        var identity = new ClaimsIdentity(
            claims: [
                new Claim(ClaimTypes.NameIdentifier, Guid.AllBitsSet.ToString()),
                new Claim(ClaimTypes.Name, "Bot"),
                new Claim("role", nameof(UserRole.Guest)),
                new Claim("role", nameof(UserRole.User)),
                new Claim("role", nameof(UserRole.Publisher)),
                new Claim("role", nameof(UserRole.Authorized)),
                new Claim("role", nameof(UserRole.Admin)),
            ],
            authenticationType: Scheme.Name);
        var principal = new GenericPrincipal(identity, roles: null);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
    
    bool IsPublicEndpoint()
        => Context.GetEndpoint()?.Metadata.OfType<AllowAnonymousAttribute>().Any() is null or true;
}