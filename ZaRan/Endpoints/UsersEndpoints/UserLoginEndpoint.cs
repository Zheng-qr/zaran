using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Riok.Mapperly.Abstractions;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Services;

namespace ZaRan.Endpoints.UsersEndpoints;

public class UserLoginEndpoint(AppDbContext dbContext, IPasswordHasher<User> passwordHasher)
    : Endpoint<UserLoginRequest, Results<Ok<UserLoginResponse>, UnauthorizedHttpResult, ForbidHttpResult>>
{
    public override void Configure()
    {
        Post("/user/login");
        AllowAnonymous(); // 允许匿名访问，绝对正确
    }

    public override async Task<Results<Ok<UserLoginResponse>, UnauthorizedHttpResult, ForbidHttpResult>> ExecuteAsync(UserLoginRequest req, CancellationToken ct)
    {
        // 🟢 调试日志：打印前端传入的账号
        Console.WriteLine($"[登录调试] 收到登录请求，账号：{req.Account}");

        // 查询用户（用户名/邮箱匹配）
        var dbUser = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == req.Account || u.UserName == req.Account, cancellationToken: ct);

        // 🟢 调试日志：打印是否找到用户
        if (dbUser is null)
        {
            Console.WriteLine($"[登录调试] ❌ 数据库中未找到该用户！");
            return TypedResults.Unauthorized();
        }

        // 验证密码
        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(dbUser, dbUser.HashedPassword, req.Password);
        
        // 🟢 调试日志：打印密码验证结果
        Console.WriteLine($"[登录调试] 密码验证结果：{passwordVerificationResult}");
        
        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            Console.WriteLine($"[登录调试] ❌ 密码错误！");
            return TypedResults.Unauthorized();
        }

        // 检查是否被封禁
        if (dbUser.IsBanned)
        {
            Console.WriteLine($"[登录调试] ❌ 账号已被封禁！");
            return TypedResults.Forbid();
        }
        
        // 登录成功 → 生成Token
        var token = JwtBearer.CreateToken(option =>
        {
            option.ExpireAt = DateTime.UtcNow.AddDays(1);
            option.User.Roles.Add("Manager", "Auditor");
            option.User.Claims.Add((ClaimTypes.NameIdentifier, dbUser.Id.ToString()));
            option.User.Claims.Add((ClaimTypes.Name, dbUser.UserName));
            
            if (dbUser.UserRole >= UserRole.Guest)
                option.User.Roles.Add(nameof(UserRole.Guest));
            if (dbUser.UserRole >= UserRole.User)
                option.User.Roles.Add(nameof(UserRole.User));
            if (dbUser.UserRole >= UserRole.Publisher)
                option.User.Roles.Add(nameof(UserRole.Publisher));
            if (dbUser.UserRole >= UserRole.Authorized)
                option.User.Roles.Add(nameof(UserRole.Authorized));
            if (dbUser.UserRole >= UserRole.Admin)
                option.User.Roles.Add(nameof(UserRole.Admin));
        });

        // 更新最后登录时间
        dbUser.LastLoginAt = DateTimeOffset.Now;
        dbContext.Users.Update(dbUser);
        await dbContext.SaveChangesAsync(cancellationToken: ct);

        // 返回结果
        var resp = UserLoginResponse.Map(dbUser);
        resp.Token = token;
        
        Console.WriteLine($"[登录调试] ✅ 登录成功！生成Token");
        return TypedResults.Ok(resp);
    }
}

// 请求模型
public class UserLoginRequest
{
    public required string Account { get; set; }
    public required string Password { get; set; }
}

// 响应模型
[Mapper]
public partial class UserLoginResponse : UserDetailResponse
{
    public string? Token { get; set; }
    
    [MapperIgnoreSource(nameof(User.Email))]
    [MapperIgnoreTarget(nameof(UserLoginResponse.Token))]
    public new static partial UserLoginResponse Map(User req);
}