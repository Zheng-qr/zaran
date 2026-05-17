using System;
using Riok.Mapperly.Abstractions;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Abstraction.Models.Dtos;
using ZaRan.Endpoints.UsersEndpoints;

namespace ZaRan.Endpoints.CommentsEndpoints;

[Mapper]
public partial class CommentDetailResponse : ResponseBase
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid TargetId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public UserDetailResponse Sender { get; set; } = null!;

    [MapperIgnoreSource(nameof(Comment.SenderId))]
    public static partial CommentDetailResponse Map(Comment comment);
}
