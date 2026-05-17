using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ZaRan.Abstraction.Models.DbModels;

[PrimaryKey(nameof(GoodId), nameof(Index))]
public class Collect
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public User? User { get; set; }
    public Good? Good { get; set; } 
    public Transaction? Transaction { get; set; }
    public required Guid GoodId { get; set; }
    public required Guid UserId { get; set; }
    public required int Index { get; set; }
    
    public required Guid TransactionId { get; set; }
}