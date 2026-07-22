using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.BuildingBlocks.Localization;

public static class LocalizationExtensions
{
    public static readonly CultureInfo[] SupportedCultures =
    [
        new("tr-TR"),
        new("en-US")
    ];

    public static IServiceCollection AddMarketplaceLocalization(this IServiceCollection services)
    {
        services.AddLocalization(options => options.ResourcesPath = "Resources");

        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture("tr-TR");
            options.SupportedCultures = SupportedCultures;
            options.SupportedUICultures = SupportedCultures;
            options.RequestCultureProviders =
            [
                new AcceptLanguageHeaderRequestCultureProvider()
            ];
        });

        return services;
    }

    public static IApplicationBuilder UseMarketplaceLocalization(this IApplicationBuilder app)
    {
        return app.UseRequestLocalization();
    }
}
