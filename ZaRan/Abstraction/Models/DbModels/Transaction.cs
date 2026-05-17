namespace ZaRan.Abstraction.Models.DbModels;

public class Transaction
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public User? User { get; set; }
    public User? TargetUser { get; set; }
    public Good? Good { get; set; }
    public Guid? CollectId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset UpdatedAt { get; set; }
    public double Price { get; set; }
    public string? Description { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    
    
    // Db Relationships
    public Guid UserId { get; set; }
    public Guid TargetUserId { get; set; }
    public Guid GoodId { get; set; }
}

public enum TransactionStatus
{
    Pending = 0,
    Completed = 1,
    Cancelled = 2
}