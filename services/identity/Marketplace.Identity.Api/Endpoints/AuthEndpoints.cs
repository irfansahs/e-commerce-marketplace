using System.Security.Claims;
using Marketplace.BuildingBlocks.Localization;
using Marketplace.Identity.Api.Auth;
using Marketplace.Identity.Api.Data;
using Marketplace.Identity.Api.Messaging;
using Marketplace.Identity.Api.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Marketplace.Identity.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/auth");

        group.MapPost("/register", RegisterAsync);
        group.MapPost("/login", LoginAsync);
        group.MapGet("/me", MeAsync).RequireAuthorization();

        return endpoints;
    }

    private static async Task<IResult> RegisterAsync(
        RegisterRequest request,
        IdentityDbContext db,
        JwtTokenService tokenService,
        IIntegrationEventPublisher publisher,
        IStringLocalizer<IdentityResources> localizer,
        ILoggerFactory loggerFactory,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var logger = loggerFactory.CreateLogger("Marketplace.Identity.Auth");
        var validation = ValidateRegister(request, localizer);
        if (validation is not null)
        {
            AuthAudit.RegisterFail(logger, request.Email?.Trim() ?? "", "validation");
            return validation;
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        if (await db.Users.AnyAsync(u => u.Email == normalizedEmail, cancellationToken))
        {
            AuthAudit.RegisterFail(logger, normalizedEmail, "email_taken");
            return MarketplaceProblemDetails.FromLocalizer(
                "IDENTITY_EMAIL_TAKEN",
                StatusCodes.Status409Conflict,
                localizer);
        }

        var (hash, salt) = PasswordHasher.HashPassword(request.Password);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            PasswordHash = hash,
            PasswordSalt = salt,
            Role = UserRole.Buyer,
            PreferredCulture = CultureHelper.ResolvePreferredCulture(request.PreferredCulture, httpContext),
            CreatedAtUtc = DateTime.UtcNow
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        await publisher.PublishUserRegisteredAsync(
            new UserRegisteredIntegrationEvent(
                user.Id,
                user.Email,
                user.Role.ToString(),
                user.PreferredCulture,
                DateTime.UtcNow),
            cancellationToken);

        AuthAudit.RegisterOk(logger, user.Email, user.Id);

        var token = tokenService.CreateAccessToken(user);
        return Results.Created(
            $"/auth/me",
            new RegisterResponse(user.Id, user.Email, user.Role.ToString(), user.PreferredCulture, token.Token, token.ExpiresInSeconds));
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        IdentityDbContext db,
        JwtTokenService tokenService,
        IStringLocalizer<IdentityResources> localizer,
        ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var logger = loggerFactory.CreateLogger("Marketplace.Identity.Auth");

        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            AuthAudit.LoginFail(logger, request.Email?.Trim() ?? "", "validation");
            return MarketplaceProblemDetails.FromLocalizer(
                "IDENTITY_VALIDATION_FAILED",
                StatusCodes.Status400BadRequest,
                localizer);
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);
        if (user is null || !PasswordHasher.Verify(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            // Same client message either way; reason is for internal audit only.
            AuthAudit.LoginFail(logger, normalizedEmail, user is null ? "unknown_user" : "bad_password");
            return MarketplaceProblemDetails.FromLocalizer(
                "IDENTITY_INVALID_CREDENTIALS",
                StatusCodes.Status401Unauthorized,
                localizer);
        }

        AuthAudit.LoginOk(logger, user.Email, user.Id, user.Role.ToString());

        var token = tokenService.CreateAccessToken(user);
        return Results.Ok(new LoginResponse(token.Token, token.ExpiresInSeconds));
    }

    private static IResult MeAsync(ClaimsPrincipal user)
    {
        var sub = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
        var email = user.FindFirstValue(ClaimTypes.Email) ?? user.FindFirstValue("email");
        var role = user.FindFirstValue(ClaimTypes.Role) ?? user.FindFirstValue("role");

        if (sub is null || email is null)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(new MeResponse(Guid.Parse(sub), email, role ?? UserRole.Buyer.ToString()));
    }

    private static IResult? ValidateRegister(RegisterRequest request, IStringLocalizer<IdentityResources> localizer)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
        {
            return MarketplaceProblemDetails.FromLocalizer(
                "IDENTITY_VALIDATION_EMAIL",
                StatusCodes.Status400BadRequest,
                localizer);
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
        {
            return MarketplaceProblemDetails.FromLocalizer(
                "IDENTITY_VALIDATION_PASSWORD",
                StatusCodes.Status400BadRequest,
                localizer);
        }

        return null;
    }

    public sealed record RegisterRequest(string Email, string Password, string? PreferredCulture);
    public sealed record LoginRequest(string Email, string Password);
    public sealed record RegisterResponse(Guid UserId, string Email, string Role, string PreferredCulture, string AccessToken, int ExpiresIn);
    public sealed record LoginResponse(string AccessToken, int ExpiresIn);
    public sealed record MeResponse(Guid UserId, string Email, string Role);
}
