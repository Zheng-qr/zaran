namespace ZaRan.Abstraction.Models.DbModels;

public class Collection
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Color { get; set; }
    public string[]? Tags { get; set; }
    public CollectionType Type { get; set; }
    public Guid[] ChildrenIds { get; set; } = [];

    public User Creator { get; set; }
    
    // Db Relationships
    public Guid CreatorId { get; set; }
}

public enum CollectionType
{
    Story,
    Article,
    Good,
    User
}