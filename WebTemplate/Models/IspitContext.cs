namespace WebTemplate.Models;

public class IspitContext : DbContext
{
    // DbSet kolekcije!
    public DbSet<User> Users { get; set; }

    public IspitContext(DbContextOptions options) : base(options)
    {
        
    }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<User>(entity =>{
        entity.HasIndex(e=>e.Username).IsUnique();
    });
  }
}
