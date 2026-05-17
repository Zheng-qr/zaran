using Riok.Mapperly.Abstractions;

namespace ZaRan.Abstraction.Models.DbModels;

public class User
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string UserName { get; set; }
    public required string NickName { get; set; }
    public string? Signature { get; set; }
    public required string Email { get; set; }
    public double Balance { get; set; } = 0.0;
    public required UserRole UserRole { get; set; }

    [MapperIgnore]
    public string? HashedPassword { get; set; }
    public string? Avatar { get; set; }
    public UserStatus UserStatus { get; set; }
    public DateTimeOffset? UnbanTime { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset LastLoginAt { get; set; }

    /// <summary>
    /// Checks if the user is currently banned (UnbanTime is set and in the future)
    /// </summary>
    public bool IsBanned => UnbanTime.HasValue && UnbanTime.Value > DateTimeOffset.UtcNow;
}

public enum UserRole
{
    Guest = 0,
    User = 1,
    Publisher = 2,
    Authorized = 3,
    Admin = 4
}

public enum UserStatus
{
    Unverified = 0,
    Normal = 1,
}