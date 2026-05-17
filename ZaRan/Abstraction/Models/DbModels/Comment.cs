namespace ZaRan.Abstraction.Models.DbModels;

public class Comment
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Content { get; set; } = string.Empty;
    public Guid TargetId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    public User? Sender { get; set; }
    
    // Db Relationships
    public Guid SenderId { get; set; }
}