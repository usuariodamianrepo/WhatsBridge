using System.Globalization;
using Clone.Web.Configuration;
using Microsoft.AspNetCore.Localization;

namespace Clone.Web.Extensions
{
    public static class LocalizationExtensions
    {
        public static void AddLocalizationConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration
                .GetSection(LocalizationSettings.SectionName)
                .Get<LocalizationSettings>() ?? new LocalizationSettings();

            var supportedCultures = settings.SupportedCultures.Length > 0
                ? settings.SupportedCultures
                : new[] { settings.DefaultCulture };

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(settings.DefaultCulture);
                options.SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
                options.SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
                // No request providers: the culture is always the one configured in appsettings.
                options.RequestCultureProviders.Clear();
            });

            // Blazor Server interactive circuits run outside the request pipeline, so the
            // configured culture is applied process-wide for both formatting and resources.
            var defaultCulture = new CultureInfo(settings.DefaultCulture);
            CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
            CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;
        }
    }
}
