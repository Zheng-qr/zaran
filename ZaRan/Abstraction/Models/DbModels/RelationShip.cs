namespace ZaRan.Abstraction.Models.DbModels;

public class RelationShip
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required User Actor { get; set; }
    
    public required Guid Target { get; set; }
    public required RelationShipType Type { get; set; } = RelationShipType.Unspecified;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    // Db Relationships
    public Guid ActorId { get; set; }
}

public enum RelationShipType
{
    Unspecified = 0,
    Follow = 1,
    ArticleLike = 2,
    GoodLike = 3,
    GoodCart = 4,
    CommentLike = 5,
}

