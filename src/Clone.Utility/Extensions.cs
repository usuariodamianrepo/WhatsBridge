using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace Clone.Utility
{
    public static class Extensions
    {
        #region String Extensions
        public static string ToId(this int value) => value.ToString("0000000");

        public static string ToMoney(this decimal value) => value.ToString("C2", new CultureInfo("es-AR"));
        public static string ToMoney(this double value) => value.ToString("C2", new CultureInfo("es-AR"));
        public static string ToPercent(this decimal value) => value.ToString("P2", new CultureInfo("es-AR"));
        public static string ToPercent(this double value) => value.ToString("P2", new CultureInfo("es-AR"));
        public static string ToDateHHmmss(this DateTime value) => value.ToString("dd/MM/yyyy HH:mm:ss");
        public static string ToDateHHmm(this DateTime value) => value.ToString("dd/MM/yy HH:mm");

        public static string ToDisabledString(this bool value) => value ? "" : "disabled";
        public static string ToYesNoString(this bool value) => value ? "Sí" : "No";

        public static string GetDisplayName(this Enum value)
        {
            if (value == null) return string.Empty;
            var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            if (member == null) return value.ToString();

            var display = member.GetCustomAttribute<DisplayAttribute>();
            if (display != null && !string.IsNullOrEmpty(display.Name))
                return display.Name;

            var desc = member.GetCustomAttribute<DescriptionAttribute>();
            if (desc != null && !string.IsNullOrEmpty(desc.Description))
                return desc.Description;

            return value.ToString();
        }
        #endregion

        #region DateTime Extensions
        public static DateTime OnlyDateHHmm(this DateTime value) 
            => DateTime.ParseExact(value.ToString("dd/MM/yyyy HH:mm"), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

        public static DateTime ConvertToShow(this DateTime value)
        {
            // We ensure that the entry date is treated as UTC.
            DateTime utcDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);

            // We search for the time zone (IANA works natively in .NET Core on Linux/Docker)
            TimeZoneInfo userTz = TimeZoneInfo.FindSystemTimeZoneById(Iana.MyZone);

            return TimeZoneInfo.ConvertTimeFromUtc(utcDate, userTz);
        }

        public static DateTime ConvertToSave(this DateTime value)
        {
            TimeZoneInfo userTz = TimeZoneInfo.FindSystemTimeZoneById(Iana.MyZone);

            return TimeZoneInfo.ConvertTimeToUtc(value, userTz);
        }

        #endregion

        #region Int Extensions
        public static IEnumerable<int> WhereGreaterThan(this IEnumerable<int> source, int threshold)
            => source.Where(x => x > threshold);
        #endregion
    }
}
