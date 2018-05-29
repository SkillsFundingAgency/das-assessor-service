using System;
using SFA.DAS.AssessorService.Domain.Extensions;

namespace SFA.DAS.AssessorService.EpaoImporter.Helpers
{
    public class ScheduledDates
    {
        public DateTime GetNextScheduledDate(DateTime todaysDate, 
            DateTime scheduledDate, DayOfWeek dayOfWeek)
        {
            if (todaysDate.Date == scheduledDate.Date)
                return scheduledDate.AddDays(7);

            while (scheduledDate.GetNextWeekday(dayOfWeek) <= todaysDate)
            {
                scheduledDate = scheduledDate.AddDays(7);
            }

            return scheduledDate;
        }

        public DateTime GetThisScheduledDate(DateTime todaysDate, DateTime scheduledDate)
        {
            if (todaysDate.Date == scheduledDate.Date)
                return scheduledDate.AddDays(7);

            while (scheduledDate.GetNextWeekday(DayOfWeek.Monday) <= todaysDate)
            {
                scheduledDate = scheduledDate.AddDays(7);
            }

            return scheduledDate.AddDays(-7);
        }

        public static DayOfWeek GetDayOfWeek(int dayOfWeek)
        {
            return (DayOfWeek) dayOfWeek;
        }
    }
}
