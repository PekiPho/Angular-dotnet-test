namespace WebTemplate.Models;

public class IspitContext : DbContext
{
    // DbSet kolekcije!
    public DbSet<User> Users { get; set; }

    public DbSet<Community> Communities {get;set;}

    public DbSet<Post> Posts{get;set;}

    public DbSet<Repost> Reposts { get; set; }

    public DbSet<Comment> Comments { get; set; }

    public DbSet<Media> Media { get; set; }

    public DbSet<Vote> Votes { get; set; }

    public IspitContext(DbContextOptions options) : base(options)
    {
        
    }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<User>(entity =>
    {
        entity.HasIndex(e => e.Username).IsUnique();
    });

    modelBuilder.Entity<Community>(entity =>
    {
        entity.HasIndex(e => e.Name).IsUnique();
    });

    modelBuilder.Entity<User>()
        .HasMany(u => u.Subscribed)
        .WithMany(c => c.Subscribers)
        .UsingEntity<Dictionary<string, object>>(
            "UserSubscribed",
            a => a.HasOne<Community>().WithMany().HasForeignKey("CommunityID"),
            a => a.HasOne<User>().WithMany().HasForeignKey("UserID")
        );

    modelBuilder.Entity<User>()
        .HasMany(u => u.Moderator)
        .WithMany(c => c.Moderators)
        .UsingEntity<Dictionary<string, object>>(
            "UserModerating",
            a => a.HasOne<Community>().WithMany().HasForeignKey("CommunityID"),
            a => a.HasOne<User>().WithMany().HasForeignKey("UserID")
        );

    //need to test this tmrw
    // modelBuilder.Entity<Comment>()
    //     .HasOne(c=>c.ReplyTo)
    //     .WithMany(c=>c.Replies)
    //     .OnDelete(DeleteBehavior.SetNull);
}

  
 
}
