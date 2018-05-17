using System;
using FluentAssertions;
using FluentDateTime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.UnitTests.GetNextDateExtension
{
    [TestClass]
    public class WhenGetPreviousWeekDateForWednesday
    {
        [TestMethod]
        public void CompoundDateTest()
        {         
            for (var firstDay = 16; firstDay <= 27; firstDay++)
            {
                var scheduledDateTime = new DateTime(2018, 05, firstDay);
                var newDate = scheduledDateTime.Previous(DayOfWeek.Wednesday);
                if (scheduledDateTime.DayOfWeek == DayOfWeek.Wednesday)
                    newDate.Date.Should().Be(scheduledDateTime.AddDays(-7).Date);
                else
                {
                    newDate.Date.Should().Be(scheduledDateTime.Day < 24
                        ? new DateTime(2018, 05, 16)
                        : new DateTime(2018, 05, 23));
                }

                Console.WriteLine($"{firstDay} =  {newDate}");
            }
        }
    }
}
