using System;
using FluentAssertions;
using FluentDateTime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SFA.DAS.AssessorService.EpaoImporter.Extensions;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.UnitTests.GetNextDateExtension
{
    [TestClass]
    public class WhenGetNextWeekDate
    {
        [TestMethod]
        public void CompoundDateTest()
        {
            var firstDay = 14;

            for (firstDay = 14; firstDay <= 25; firstDay++)
            {
                var scheduledDateTime = new DateTime(2018, 05, firstDay);
                var newDate = scheduledDateTime.Next(DayOfWeek.Monday);
                if (scheduledDateTime.DayOfWeek == DayOfWeek.Monday)
                    newDate.Date.Should().Be(scheduledDateTime.AddDays(7).Date);
                else
                {
                    newDate.Date.Should().Be(scheduledDateTime.Day < 22
                        ? new DateTime(2018, 05, 21)
                        : new DateTime(2018, 05, 28));
                }

                Console.WriteLine($"{firstDay} =  {newDate}");
            }
        }
    }
}
