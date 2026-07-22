namespace Marketplace.Identity.Api.Auth;

/// <summary>
/// Structured security audit events for Graylog (Identity owns auth — not Gateway).
/// Never log passwords or tokens.
/// </summary>
public static class AuthAudit
{
    public const string LoginSucceeded = "identity.auth.login_succeeded";
    public const string LoginFailed = "identity.auth.login_failed";
    public const string RegisterSucceeded = "identity.auth.register_succeeded";
    public const string RegisterFailed = "identity.auth.register_failed";

    public static void LoginOk(ILogger logger, string email, Guid userId, string role) =>
        logger.LogInformation(
            "Auth event {EventType} for {Email} user {UserId} role {Role}",
            LoginSucceeded,
            email,
            userId,
            role);

    public static void LoginFail(ILogger logger, string email, string reason) =>
        logger.LogWarning(
            "Auth event {EventType} for {Email} reason {Reason}",
            LoginFailed,
            email,
            reason);

    public static void RegisterOk(ILogger logger, string email, Guid userId) =>
        logger.LogInformation(
            "Auth event {EventType} for {Email} user {UserId}",
            RegisterSucceeded,
            email,
            userId);

    public static void RegisterFail(ILogger logger, string email, string reason) =>
        logger.LogWarning(
            "Auth event {EventType} for {Email} reason {Reason}",
            RegisterFailed,
            email,
            reason);
}
