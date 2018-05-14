using System;
using SFA.DAS.AssessorService.EpaoImporter.Extensions;

namespace SFA.DAS.AssessorService.EpaoImporter.Helpers
{
    public class ScheduledDates
    {
        public DateTime GetNextScheduledDate(DateTime todaysDate, DateTime scheduledDate)
        {
            if (todaysDate.Date == scheduledDate.Date)
                return scheduledDate.AddDays(7);

            while (scheduledDate.GetNextWeekday(DayOfWeek.Monday) <= todaysDate)
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
    }
}
