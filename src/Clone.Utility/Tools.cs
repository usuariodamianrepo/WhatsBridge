using System.Globalization;
using System.Text.RegularExpressions;

namespace Clone.Utility
{
    public class Tools
    {
        public static string CleanInput(string strIn)
        {
            return strIn.Replace(" [ ", " ").Replace(" ^ ", " ").Replace(" & ", " ").Replace(" . ", " ")
                .Replace(" ¿ ", " ").Replace(" * ", " ").Replace(" ] ", " ").Replace(" ' ", " ")
                .Replace(" < ", " ").Replace(" > ", " ").Replace(" - ", " ").Replace(" _ ", " ")
                .Replace(" | ", " ").Replace(" ? ", " ").Replace(" ; ", " ").Replace(" " + (char)34 + " ", " ")
                .Replace(" " + (char)92 + " ", " ").Replace(" , ", " ").Replace(" { ", " ")
                .Replace(" } ", " ").Replace(" $ ", " ").Replace(" # ", " ").Replace(" @ ", " ")
                .Replace(" + ", " ").Replace(" / ", " ").Replace(" ( ", " ").Replace(" ) ", " ")
                .Replace(" = ", " ").Replace(" % ", " ").Replace(" · ", " ").Replace(" ! ", " ")
                .Replace(" ¡ ", " ");
        }

        public static string[] ConvertGuidArrayToString(Guid[] guids)
        {
            List<string> l = new List<string>();
            foreach (Guid item in guids)
                l.Add(item.ToString());
            return l.ToArray();
        }

        /// <summary>
        /// Retorna la edad para 2 fechas ingresadas
        /// </summary>
        /// <param name="dateOfBirth">fecha de nacimiento</param>
        /// <param name="dateCompare">fecha que deseas saber la edad</param>
        /// <returns></returns>
        public static int GetAge(DateTime? dateOfBirth, DateTime dateCompare)
        {
            if (dateOfBirth == null)
                return 0;

            var date = Convert.ToDateTime(dateOfBirth);

            var a = (dateCompare.Year * 100 + dateCompare.Month) * 100 + dateCompare.Day;
            var b = (date.Year * 100 + date.Month) * 100 + date.Day;

            return (a - b) / 10000;
        }
        /// <summary>
        /// Retorna en dias la diferencia entre dos fechas
        /// </summary>
        /// <param name="from">fecha desde</param>
        /// <param name="to">fecha hasta</param>
        /// <returns></returns>
        public static int GetDateDiffInDays(DateTime from, DateTime to)
        {
            TimeSpan ts = to - from;
            int days = ts.Days;
            return days;
        }

        public static DateTime GetDateFrom(DateTime from, DateTime? time)
        {
            if (time.HasValue)
                return new DateTime(from.Year, from.Month, from.Day, time.Value.Hour, time.Value.Minute, 0);

            return new DateTime(from.Year, from.Month, from.Day, 0, 0, 0);
        }

        public static DateTime GetDateTo(DateTime from, DateTime? time)
        {
            if (time.HasValue)
                return new DateTime(from.Year, from.Month, from.Day, time.Value.Hour, time.Value.Minute, 59);

            return new DateTime(from.Year, from.Month, from.Day, 23, 59, 59);
        }

        public static string GetDayName(int dia)
        {
            CultureInfo ci = new CultureInfo("Es-Es");
            return ci.DateTimeFormat.GetDayName((DayOfWeek)dia).ToUpper();
        }

        public static bool IsGuid(string guid)
        {
            Guid tempGuid;
            return Guid.TryParse(guid, out tempGuid);
        }

        public static bool IsGuid(object s)
        {
            return IsGuid(s.ToString());
        }

        /// <summary>
        /// Deja nada mas que las letras
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string NormalizeChar(string s)
        {
            return Regex.Replace(s, "[^a-zA-Z]+", "", RegexOptions.Compiled);
        }

        /// <summary>
        /// Deja nada mas que las letras y nros
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string NormalizeCharNumber(string s)
        {
            return Regex.Replace(s, "[^a-zA-Z0-9]+", "", RegexOptions.Compiled);
        }

        /// <summary>
        /// Deja nada mas que las numeros
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string NormalizeNumber(string s)
        {
            return Regex.Replace(s, "[^0-9]+", "", RegexOptions.Compiled); ;
        }

        public static string Substring(string str, int length)
        {
            if (str.Length > length)
                return (str.Substring(0, length));
            else
                return str;
        }

        public static string SubstringLeft(string str, int length)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > length)
                return (str.Substring(0, length)).Trim();
            else
                return str;
        }

        public static string SubstringLeftChar(string str, int length)
        {
            if (str.Length > length)
                return (str.Substring(str.IndexOf('-') + 1, length));
            else
                return str;
        }
        public static string SubstringRight(string str, int length)
        {
            if (str.Length > length)
                return (str.Substring(str.Length - length, length)).Trim();
            else
                return str;
        }
    }
}
