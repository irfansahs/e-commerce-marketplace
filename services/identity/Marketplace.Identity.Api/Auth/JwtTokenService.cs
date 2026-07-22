using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Marketplace.Identity.Api.Data;
using Microsoft.IdentityModel.Tokens;

namespace Marketplace.Identity.Api.Auth;

public sealed class JwtTokenService(IConfiguration configuration)
{
    public (string Token, int ExpiresInSeconds) CreateAccessToken(User user)
    {
        var jwt = configuration.GetSection("Jwt");
        var signingKey = jwt["SigningKey"]
            ?? throw new InvalidOperationException("Jwt:SigningKey is not configured.");

        var expiresMinutes = jwt.GetValue("ExpirationMinutes", 60);
        var expires = DateTime.UtcNow.AddMinutes(expiresMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString()),
            new("role", user.Role.ToString())
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"] ?? "marketplace-identity",
            audience: jwt["Audience"] ?? "marketplace-api",
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), (int)TimeSpan.FromMinutes(expiresMinutes).TotalSeconds);
    }
}
