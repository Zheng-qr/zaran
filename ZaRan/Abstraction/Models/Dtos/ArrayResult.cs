using System.Text.Json.Serialization;
using ZaRan.Endpoints.ArticlesEndpoints;
using ZaRan.Endpoints.UsersEndpoints;

namespace ZaRan.Abstraction.Models.Dtos;

public class ArrayResult<T> where T : ResponseBase
{

    public ArrayResult()
    {
        
    }
    
    public ArrayResult(T[] items, int total)
    {
        Items = items;
        Total = total;
    }
    
    public int Total { get; set; }
    public T[] Items { get; set; } = [];
}

public abstract class ResponseBase
{
    
}