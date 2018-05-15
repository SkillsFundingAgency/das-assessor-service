using System;
using FluentAssertions;
using FluentDateTime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.UnitTests.GetNextDateExtension
{
    [TestClass]
    public class WhenGetPreviousWeekDateForMonday
    {
        [TestMethod]
        public void CompundDateTest()
        {
            var firstDay = 14;

            for (firstDay = 14; firstDay <= 25; firstDay++)
            {
                var scheduledDateTime = new DateTime(2018, 05, firstDay);
                var newDate = scheduledDateTime.Previous(DayOfWeek.Monday);
                if (scheduledDateTime.DayOfWeek == DayOfWeek.Monday)
                    newDate.Date.Should().Be(scheduledDateTime.AddDays(-7).Date);
                else
                {
                    newDate.Date.Should().Be(scheduledDateTime.Day < 22
                        ? new DateTime(2018, 05, 14)
                        : new DateTime(2018, 05, 21));
                }

                Console.WriteLine($"{firstDay} =  {newDate}");
            }
        }
    }
}
