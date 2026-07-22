namespace Marketplace.Identity.Api.Data;

public enum UserRole
{
    Buyer = 0,
    Seller = 1,
    Admin = 2
}

public sealed class User
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string PasswordSalt { get; set; }
    public UserRole Role { get; set; }
    public string PreferredCulture { get; set; } = "tr-TR";
    public DateTime CreatedAtUtc { get; set; }
}
