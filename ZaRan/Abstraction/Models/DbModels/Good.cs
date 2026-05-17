namespace ZaRan.Abstraction.Models.DbModels;

public class Good
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Name { get; set; } = string.Empty;
    public double Price { get; set; }
    public int Stoke { get; set; } = -1;
    public double DiscountedPrice { get; set; }
    public GoodStatus Status { get; set; } = GoodStatus.Draft;
    public string? PublisherName { get; set; }
    public string? ImageUrl { get; set; }
    public string? CopyrightInfo { get; set; } = string.Empty; // 版权信息字段
    public User? Publisher { get; set; }
    public Article? RelatedArticle { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
    public Collection? Collection { get; set; }

    // Db Relationships
    public Guid PublisherId { get; set; }
    public Guid RelatedArticleId { get; set; }
    public Guid CollectionId { get; set; }
}

public enum GoodStatus
{
    Draft,
    Available,
    Unavailable,
    Rejected
}