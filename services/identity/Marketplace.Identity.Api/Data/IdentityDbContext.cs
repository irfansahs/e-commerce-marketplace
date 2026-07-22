using Microsoft.EntityFrameworkCore;

namespace Marketplace.Identity.Api.Data;

public sealed class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Email).HasMaxLength(256);
            entity.Property(u => u.PasswordHash).HasMaxLength(512);
            entity.Property(u => u.PasswordSalt).HasMaxLength(128);
            entity.Property(u => u.PreferredCulture).HasMaxLength(16);
            entity.Property(u => u.Role).HasConversion<string>().HasMaxLength(32);
        });
    }
}
