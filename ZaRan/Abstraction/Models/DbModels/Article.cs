namespace ZaRan.Abstraction.Models.DbModels;

public class Article
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Title { get; set; } = string.Empty;
    public required ArticleStatus Status { get; set; }
    public required ArticleType Type { get; set; }
    public string? Body { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string[] Tags { get; set; } = [];
    public string? Color { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageSmallUrl { get; set; }
    public User? Author { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
    
    // Db Relationships
    public Guid AuthorId { get; set; }
}

public enum ArticleStatus
{
    Draft,
    Published,
    Banned,
    Archived,
    Rejected
}

public enum ArticleType
{
    Story,
    Character,
    Gene,
    Wiki,
    Post,
    Pattern
}