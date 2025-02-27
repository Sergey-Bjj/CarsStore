using CarsStore.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
public class CarStoreDbContext : IdentityDbContext<User>
{
    public DbSet<Car> Cars { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public CarStoreDbContext(DbContextOptions<CarStoreDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<User>().ToTable("AspNetUsers");
        
        builder.Entity<User>()
            .HasMany(u => u.Cars)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade); 

        builder.Entity<Car>()
            .ToTable("Cars")
            .HasKey(c => c.Id);
        
        builder.Entity<User>()
            .HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<RefreshToken>()
            .ToTable("RefreshTokens")
            .HasKey(rt => rt.Id);
        
        builder.Entity<RefreshToken>()
            .HasIndex(rt => new { rt.UserId, rt.Token });
    }
}
