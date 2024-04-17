namespace Calender.Console
{
    internal static class DateTimeHelper
    {
        public static DateTime? ParseDateTime(string input)
        {
            if (DateTime.TryParseExact(input, "dd/MM", null, System.Globalization.DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
            return null;
        }

        public static DateTime? ParseTime(string input)
        {
            if (DateTime.TryParseExact(input, "HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
            return null;
        }
    }
}