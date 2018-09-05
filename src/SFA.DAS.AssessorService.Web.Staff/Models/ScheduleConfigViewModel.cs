using SFA.DAS.AssessorService.Web.Staff.Domain;
using SFA.DAS.AssessorService.Web.Staff.Helpers.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.AssessorService.Web.Staff.Models
{
    public class ScheduleConfigViewModel
    {
        public Guid Id { get; set; }
        public DateTime RunTime { get; set; }
        public string Interval { get; set; }
        public ScheduleInterval? ScheduleInterval { get; set; }
        public bool IsRecurring { get; set; }
        public ScheduleJobType ScheduleType { get; set; }

        [FutureDate(GracePeriodInMinutes = 5, ErrorMessage = "Please enter a future date")]
        public DateTime Date => DateTime.MinValue.AddYears(Year).AddMonths(Month).AddDays(Day).AddHours(Hour).AddMinutes(Minute);
        [Range(1, 31, ErrorMessage = "Please enter a valid Day")]
        public int Day { get; set; }
        [Range(1, 12, ErrorMessage = "Please enter a valid Month")]
        public int Month { get; set; }
        [Range(2018, 9999, ErrorMessage = "Please enter a valid Year")]
        public int Year { get; set; }
        [Range(0, 23, ErrorMessage = "Please enter a valid Hour")]
        public int Hour { get; set; }
        [Range(0, 59, ErrorMessage = "Please enter a valid Minute")]
        public int Minute { get; set; }
    }
}
