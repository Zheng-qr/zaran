using Riok.Mapperly.Abstractions;
using ZaRan.Abstraction.Models.DbModels;
using ZaRan.Abstraction.Models.Dtos;
using ZaRan.Endpoints.UsersEndpoints;

namespace ZaRan.Endpoints.CollectionsEndpoints;

[Mapper]
public partial class CollectionDetailResponse : ResponseBase
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Color { get; set; }
    public string[]? Tags { get; set; }
    public CollectionType Type { get; set; }
    public Guid[] ChildrenIds { get; set; } = [];
    public UserDetailResponse? Creator { get; set; }
    public Guid CreatorId { get; set; }

    [MapperIgnoreSource(nameof(Collection.CreatorId))]
    public static partial CollectionDetailResponse Map(Collection collection);
}
