namespace SFA.DAS.AssessorService.TestHelper
{
    public static class DateTimeExtensions
    {
        public static DateTime GetEndOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond)
                 .AddMonths(1)
                 .AddDays(-1);
        }
    }

}
