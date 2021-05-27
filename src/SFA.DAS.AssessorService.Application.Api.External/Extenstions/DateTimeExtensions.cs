using System;

namespace SFA.DAS.AssessorService.Application.Api.External.Extenstions
{
    public static class DateTimeExtensions
    {
        public static DateTime? DropMilliseconds(this DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.DropMilliseconds() : dateTime;   
        }

        public static DateTime DropMilliseconds(this DateTime dt)
        {
            return dt.AddMilliseconds((dt.Millisecond * -1));
            //return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
        }
    }
}
