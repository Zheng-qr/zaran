using Microsoft.EntityFrameworkCore;
using ZaRan.Abstraction.Models.DbModels;

namespace ZaRan.Services;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<RelationShip> RelationShips { get; set; }
    public DbSet<Article> Articles { get; set; }
    public DbSet<Good> Goods { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Collect> Collects { get; set; }
    public DbSet<Collection> Collections { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RelationShip>()
            .HasKey(rs => rs.Id);
        modelBuilder.Entity<RelationShip>()
            .HasOne(rs => rs.Actor)
            .WithMany()
            .HasForeignKey(rs => rs.ActorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RelationShip>()
            .HasIndex(t => t.ActorId);
        modelBuilder.Entity<RelationShip>()
            .HasIndex(t => t.Target);


        modelBuilder.Entity<Message>()
            .HasKey(t => t.Id);
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Receiver)
            .WithMany()
            .HasForeignKey(m => m.ReceiverId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Message>()
            .HasIndex(m => m.SenderId);
        modelBuilder.Entity<Message>()
            .HasIndex(m => m.ReceiverId);

        
        modelBuilder.Entity<Comment>()
            .HasKey(c => c.Id);
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Sender)
            .WithMany()
            .HasForeignKey(c => c.SenderId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Comment>()
            .HasIndex(c => c.SenderId);
        modelBuilder.Entity<Comment>()
            .HasIndex(c => c.TargetId);
        
        
        modelBuilder.Entity<Good>()
            .HasKey(g => g.Id);
        
        modelBuilder.Entity<Good>()
            .HasOne(g => g.Publisher)
            .WithMany()
            .HasForeignKey(g => g.PublisherId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Good>()
            .HasOne(g => g.RelatedArticle)
            .WithMany()
            .HasForeignKey(g => g.RelatedArticleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Good>()
            .HasOne(g => g.Collection)
            .WithMany()
            .HasForeignKey(g => g.CollectionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.SetNull);
        
        modelBuilder.Entity<Good>()
            .HasIndex(g => g.PublisherId);
        
        
        
        modelBuilder.Entity<Article>()
            .HasKey(a => a.Id);
        modelBuilder.Entity<Article>()
            .HasOne(a => a.Author)
            .WithMany()
            .HasForeignKey(a => a.AuthorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        
        // Transactions
        modelBuilder.Entity<Transaction>()
            .HasKey(t => t.Id);
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.TargetUser)
            .WithMany()
            .HasForeignKey(t => t.TargetUserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Good)
            .WithMany()
            .HasForeignKey(t => t.GoodId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Transaction>()
            .HasIndex(t => t.UserId);
        modelBuilder.Entity<Transaction>()
            .HasIndex(t => t.GoodId);
        
        
        // Collects
        modelBuilder.Entity<Collect>()
            .HasKey(c => new { c.GoodId, c.Index });
        modelBuilder.Entity<Collect>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Collect>()
            .HasOne(c => c.Good)
            .WithMany()
            .HasForeignKey(c => c.GoodId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Collect>()
            .HasOne(c => c.Transaction)
            .WithOne()
            .HasForeignKey<Collect>(c => c.TransactionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Collect>()
            .HasIndex(c => c.UserId);
        modelBuilder.Entity<Collect>()
            .HasIndex(c => c.GoodId);
        
        
        // collections
        modelBuilder.Entity<Collection>()
            .HasKey(c => c.Id);
        modelBuilder.Entity<Collection>()
            .HasOne(c => c.Creator)
            .WithMany()
            .HasForeignKey(c => c.CreatorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Collection>()
            .HasIndex(c => c.CreatorId);
        modelBuilder.Entity<Collection>()
            .HasIndex(c => c.Type);
        modelBuilder.Entity<Collection>()
            .HasIndex(c => c.Id);
        
        base.OnModelCreating(modelBuilder);
    }
}