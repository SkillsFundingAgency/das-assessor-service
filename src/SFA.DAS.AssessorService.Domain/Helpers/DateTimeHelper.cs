using System;

namespace SFA.DAS.AssessorService.Domain.Helpers
{
    public interface IDateTimeHelper
    {
        DateTime DateTimeNow { get; }
        DateTime DateTimeUtcNow { get; }
    }

    public class DateTimeHelper : IDateTimeHelper
    {
        public DateTime DateTimeNow => DateTime.Now;
        public DateTime DateTimeUtcNow => DateTime.UtcNow;
    }
}
