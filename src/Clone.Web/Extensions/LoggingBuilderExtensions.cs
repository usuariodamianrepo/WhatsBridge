using Serilog;

namespace Clone.Web.Extensions
{
    public static class LoggingBuilderExtensions
    {
        public static void AddLoggingConfiguration(this ILoggingBuilder logging, IConfiguration configuration)
        {
            var logger = new LoggerConfiguration()
                               .ReadFrom.Configuration(configuration)
                               .Enrich.FromLogContext()
                               .CreateLogger();
            logging.ClearProviders();
            logging.AddSerilog(logger);
        }
    }
}
