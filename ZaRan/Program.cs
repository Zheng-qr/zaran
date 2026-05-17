using System.Globalization;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Scalar.AspNetCore;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Services;
using ZaRan.Services.Languages;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services
    .AddAuthorization()
    .AddFastEndpoints()
    .SwaggerDocument(options =>
    {
        options.ShortSchemaNames = true;
    });

// ========== 核心修复1：JWT认证配置（移除重复注册，确保JWT为默认方案） ==========
var signingKey = "12345678901234567890123456789012";
// 先注册FastEndpoints JWT，自动设为默认方案
builder.Services.AddAuthenticationJwtBearer(option => { option.SigningKey = signingKey; });
builder.Services.Configure<JwtCreationOptions>(o => { o.SigningKey = signingKey; });

// ========== 核心修复2：Bot认证方案（单独注册，不覆盖默认JWT，修复参数错误） ==========
builder.Services.AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, BotAuthenticationHandler>(
        BotAuthenticationHandler.SchemeName,
        configureOptions: _ => { } // 必须传空配置Action，修复编译错误
    );

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseInMemoryDatabase("mem");
    // option.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddKeyedSingleton<ILanguage, ZhCnLanguage>("zh-Hans");
builder.Services.AddSingleton<ILanguage, ZhCnLanguage>();
ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("zh-Hans");

Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "static"));
builder.Services.AddKeyedSingleton<IFileProvider, PhysicalFileProvider>("static", (_, _) => new PhysicalFileProvider(
    Path.Combine(AppContext.BaseDirectory, "static")));
var app = builder.Build();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints(option =>
{
    option.Endpoints.ShortNames = true;
    option.Endpoints.RoutePrefix = "api";
}); // must before openapi mapping

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(c => c.Path = "/openapi/{documentName}.json");
    app.MapScalarApiReference(option =>
    {
        option.AddApiKeyAuthentication(JwtBearerDefaults.AuthenticationScheme,
            scheme => { scheme.Name = "Authorization"; });
        option.AddApiKeyAuthentication(BotAuthenticationHandler.SchemeName,
            scheme => { scheme.Name = BotAuthenticationHandler.HeaderName; });
        option.AddPreferredSecuritySchemes(JwtBearerDefaults.AuthenticationScheme, BotAuthenticationHandler.SchemeName);
    });
}

await app.RunAsync().ConfigureAwait(false);