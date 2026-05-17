using Microsoft.AspNetCore.Mvc;

namespace ZaRan.Abstraction.Models.Dtos;

public class PaginationRequest
{
    [FromQuery] public int Offset { get; set; } = 0;

    [FromQuery] public int Limit { get; set; } = 10;
    
    [FromQuery] public bool Desc { get; set; } = false;
}

public class SearchRequest : PaginationRequest
{
    [FromQuery] public string? Keyword { get; set; }
}

public class UserTargetSearchRequest : SearchRequest
{
    public Guid? UserId { get; set; }
}