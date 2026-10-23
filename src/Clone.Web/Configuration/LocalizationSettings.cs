namespace Clone.Web.Configuration
{
    public class LocalizationSettings
    {
        public const string SectionName = "Localization";

        public string[] SupportedCultures { get; set; } = [];

        public string DefaultCulture { get; set; } = "en-US";
    }
}
