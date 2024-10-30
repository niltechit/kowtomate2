namespace CutOutWiz.Core.Utilities
{
    public static class DateHelper
    {
        /// <summary>
        /// Converts to nullable short date string
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToNullableShortDateString(this DateTime? date)
        {
            return date.HasValue ? DateTime.Parse(date.ToString()).ToShortDateString() : string.Empty;
        }

        public static string ToNullableDateString(this DateTime? date)
        {
            return date.HasValue ? DateTime.Parse(date.ToString()).ToString() : string.Empty;
        }

        public static string ToNullableShortTimeString(this DateTime? date)
        {
            return date.HasValue ? DateTime.Parse(date.ToString()).ToShortTimeString() : string.Empty;
        }

        /// <summary>
        /// Converts date to nullable short date string
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToNullableLongDateString(this DateTime? date)
        {
            return date.HasValue ? DateTime.Parse(date.ToString()).ToLongDateString() : string.Empty;
        }

        public static string ToNullableDateTimeStringInDetailsFormat(this DateTime? date)
        {
            return date.HasValue ? DateTime.Parse(date.ToString()).ToString("dd MMM yyyy hh:mm tt") : string.Empty;
        }

        public static string ToNullableDateStringInDetailsFormat(this DateTime? date)
        {
            return date.HasValue ? DateTime.Parse(date.ToString()).ToString("dd MMM yyyy") : string.Empty;
        }

        public static DateTime ConvertUnixTimestampToDate(long unixTimestamp)
        {
            //// Convert string to long
            //if (!long.TryParse(unixTimestampString, out long unixTimestamp))
            //{
            //    throw new ArgumentException("Invalid Unix timestamp string");
            //}

            // Convert Unix timestamp to DateTime
            return DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).UtcDateTime;
        }

        public static string FormatUtcOffset(TimeSpan offset)
        {
            int hours = offset.Hours;
            int minutes = offset.Minutes;
            string sign = (hours >= 0) ? "+" : "-";
            hours = Math.Abs(hours);
            return $"{sign}{hours:D2}:{minutes:D2}";
        }

        public static TimeSpan GetUtcOffset(string timeZoneId)
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return timeZone.BaseUtcOffset;
        }

        public static string DefaultTimeZoneId()
        {
            return "Bangladesh Standard Time";
        }

        public static DateTime GetCurrentTimeByTimeZoneId(string timeZoneId)
        {
            // Get the time zone info
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
        }
    }
}
