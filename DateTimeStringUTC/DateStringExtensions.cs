namespace DateTimeStringUTC
{
    public static class DateStringExtensions
    {
        public static string TimeStringFromIsoDateString(this string isoString)
        {
            try
            {
                var parts = isoString.Split('T')[1].Split(':');
                var ampm = int.Parse(parts[0]) > 12 ? "PM" : "AM";
                return $"{parts[0].TrimStart('0'),2}:{parts[1]} {ampm}";
            }
            catch
            {
                throw new Exception("String is not valid iso format");
            }
        }

        public static DateTime ToCST(this DateTime date)
        {
            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(date.Ticks, DateTimeKind.Utc), cstZone);
            return cstTime;
        }

        public static DateTime ToDallasTime(this DateTime utcDateTime)
        {
            if (utcDateTime.Kind == DateTimeKind.Utc)
                return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, DallasTimeZoneInfo());

            if (utcDateTime.Kind == DateTimeKind.Local)
                return TimeZoneInfo.ConvertTime(utcDateTime, DallasTimeZoneInfo());


            return utcDateTime;
        }

        public static DateTime ToUtcTime(this DateTime LocalDateTime)
        {
            if (LocalDateTime.Kind == DateTimeKind.Utc)
                return LocalDateTime;

            if (LocalDateTime.Kind == DateTimeKind.Local)
                return TimeZoneInfo.ConvertTimeToUtc(LocalDateTime, DallasTimeZoneInfo());

            return LocalDateTime;
        }

        private static TimeZoneInfo DallasTimeZoneInfo()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {

                return TimeZoneInfo.FindSystemTimeZoneById("America/Chicago");
            }
            catch
            {
                throw;
            }
        }
    }
}
