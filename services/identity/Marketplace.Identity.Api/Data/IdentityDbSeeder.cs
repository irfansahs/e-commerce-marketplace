using Marketplace.Identity.Api.Auth;
using Marketplace.Identity.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Identity.Api.Data;

public static class IdentityDbSeeder
{
    public const string DevAdminEmail = "admin@marketplace.local";

    public static async Task SeedAsync(IdentityDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Users.AnyAsync(u => u.Role == UserRole.Admin, cancellationToken))
        {
            return;
        }

        var (hash, salt) = PasswordHasher.HashPassword("Admin123!");
        db.Users.Add(new User
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Email = DevAdminEmail,
            PasswordHash = hash,
            PasswordSalt = salt,
            Role = UserRole.Admin,
            PreferredCulture = "tr-TR",
            CreatedAtUtc = DateTime.UtcNow
        });

        await db.SaveChangesAsync(cancellationToken);
    }
}
