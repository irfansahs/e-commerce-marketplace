using System.Globalization;
using Marketplace.BuildingBlocks.Localization;

namespace Marketplace.Identity.Api;

public static class CultureHelper
{
    public static string ResolvePreferredCulture(string? requestedCulture, HttpContext httpContext)
    {
        if (!string.IsNullOrWhiteSpace(requestedCulture))
        {
            return Normalize(requestedCulture);
        }

        var feature = httpContext.Features.Get<Microsoft.AspNetCore.Localization.IRequestCultureFeature>();
        var culture = feature?.RequestCulture.UICulture.Name;
        return Normalize(culture);
    }

    private static string Normalize(string? culture)
    {
        if (string.IsNullOrWhiteSpace(culture))
        {
            return "tr-TR";
        }

        if (culture.StartsWith("en", StringComparison.OrdinalIgnoreCase))
        {
            return "en-US";
        }

        if (culture.StartsWith("tr", StringComparison.OrdinalIgnoreCase))
        {
            return "tr-TR";
        }

        return LocalizationExtensions.SupportedCultures
            .Select(c => c.Name)
            .FirstOrDefault(c => c.Equals(culture, StringComparison.OrdinalIgnoreCase)) ?? "tr-TR";
    }
}
