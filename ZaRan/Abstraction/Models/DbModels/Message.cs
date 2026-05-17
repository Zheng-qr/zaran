namespace ZaRan.Abstraction.Models.DbModels;

public class Message
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public required string Title { get; set; }
    public User? Sender { get; set; }
    public User? Receiver { get; set; }
    public bool IsRead { get; set; } = false;
    public required string Content { get; set; } = string.Empty;
    public MessageType Type { get; set; } = MessageType.System;

    // Db Relationships
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
}

public enum MessageType
{
    System = 0,      // 系统通知
    Order = 1,       // 订单消息
    Comment = 2,     // 评论回复
    Follow = 3,      // 关注通知
    Private = 4,     // 私信
    Review = 5       // 审核通知
}