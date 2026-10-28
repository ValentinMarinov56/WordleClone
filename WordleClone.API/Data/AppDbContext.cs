using Microsoft.EntityFrameworkCore;
using WordleClone.API.Models    ;

namespace WordleClone.API.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
    public DbSet<User> users => Set<User>();
    public DbSet<Wordl> wordls => Set<Wordl>();
    public DbSet<Review> reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) // manual db releation configuration
    {
    modelBuilder.Entity<Review>() // picking which model to configure
        .HasOne(r => r.User) // each review has one user
        .WithMany(u => u.Reviews) // a user can have many reviews
        .HasForeignKey(r => r.UserId) // what prop is the foreign key
        .OnDelete(DeleteBehavior.SetNull); // when user is deleted, dont delete the review too, set its user to null
    }
}