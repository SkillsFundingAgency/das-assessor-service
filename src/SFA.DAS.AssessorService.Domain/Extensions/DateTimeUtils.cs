using System;

namespace SFA.DAS.AssessorService.Domain.Extensions
{
    public static class DateTimeUtils
    {
        public static DateTime GetNextWeekday(this DateTime start, DayOfWeek day)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }

        public static DateTime UtcToTimeZoneTime(this DateTime time, string timeZoneId = "GMT Standard Time")
        {
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(time, tzi);
        }

        public static DateTime UtcFromTimeZoneTime(this DateTime time, string timeZoneId = "GMT Standard Time")
        {
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeToUtc(time, tzi);
        }
    }
}
