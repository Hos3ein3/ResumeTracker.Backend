using System.Globalization;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace ResumeTracker.Infrastructure.Configurations;

public static class LocalizationConfiguration
{
    public static readonly string[] SupportedCultures = ["en", "fa", "it"];
    public const string DefaultCulture = "en";

    public static IServiceCollection AddLocalizationSetup(this IServiceCollection services)
    {
        services.AddLocalization(options =>
        {
            // Root folder where the runtime looks for resx files
            // Matches src/ResumeTracker.Application/Resources/
            options.ResourcesPath = "Resources";
        });

        services.Configure<RequestLocalizationOptions>(options =>
        {
            var cultures = SupportedCultures
                .Select(c => new CultureInfo(c))
                .ToList();

            options.DefaultRequestCulture = new RequestCulture(DefaultCulture);
            options.SupportedCultures = cultures;
            options.SupportedUICultures = cultures;

            // Priority order — first match wins:
            options.RequestCultureProviders =
            [
                // 1. ?culture=fa query string
                new QueryStringRequestCultureProvider(),

                // 2. Accept-Language: fa header (sent by browsers automatically)
                new AcceptLanguageHeaderRequestCultureProvider(),

                // 3. X-Culture: fa custom header (useful for mobile/API clients)
                new CustomRequestCultureProvider(context =>
                {
                    var culture = context.Request.Headers["X-Culture"].FirstOrDefault();
                    if (!string.IsNullOrEmpty(culture)
                        && SupportedCultures.Contains(culture))
                    {
                        return Task.FromResult<ProviderCultureResult?>(
                            new ProviderCultureResult(culture));
                    }
                    return Task.FromResult<ProviderCultureResult?>(null);
                })
            ];
        });

        return services;
    }

    public static IApplicationBuilder UseLocalizationSetup(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices
            .GetRequiredService<Microsoft.Extensions.Options
                .IOptions<RequestLocalizationOptions>>().Value;

        app.UseRequestLocalization(options =>
{
    options
        //.AddSupportedCultures(SupportedCultures)
        .AddSupportedUICultures(SupportedCultures)
        .SetDefaultCulture("en")
        .AddInitialRequestCultureProvider(
            new AcceptLanguageHeaderRequestCultureProvider());
});
        return app;
    }
}