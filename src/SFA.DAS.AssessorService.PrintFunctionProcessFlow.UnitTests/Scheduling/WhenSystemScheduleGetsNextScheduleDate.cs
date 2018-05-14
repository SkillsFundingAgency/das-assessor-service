using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SFA.DAS.AssessorService.EpaoImporter.Helpers;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.UnitTests.Scheduling
{
    [TestClass]
    public class WhenSystemScheduleGetsNextScheduleDate
    {
        [TestMethod]
        public void GetNextDateForSystemSucessfullyRunsOnThe14thAndTodaysDateIsThe14th()
        {
            var scheduledDateTime = new DateTime(2018, 05, 14);

            var nextDate = new ScheduledDates().GetNextScheduledDate(new DateTime(2018, 05, 14), new DateTime(2018, 05, 14));

            nextDate.Date.Should().Be(new DateTime(2018, 05, 21));
        }

        [TestMethod]
        public void GetNextDateForSystemSucessfullyRunsOnThe14thAndTodaysDateIsThe15th()
        {
            var scheduledDateTime = new DateTime(2018, 05, 14);

            var nextDate = new ScheduledDates().GetNextScheduledDate(new DateTime(2018, 05, 15), scheduledDateTime);

            nextDate.Date.Should().Be(new DateTime(2018, 05, 21));
        }

        [TestMethod]
        public void GetNextDateForSystemSucessfullyRunsOnThe14thAndTodaysDateIsThe20th()
        {
            var scheduledDateTime = new DateTime(2018, 05, 14);

            var nextDate = new ScheduledDates().GetNextScheduledDate(new DateTime(2018, 05, 20), scheduledDateTime);

            nextDate.Date.Should().Be(new DateTime(2018, 05, 21));
        }

        [TestMethod]
        public void GetNextDateForSystemSucessfullyRunsOnThe14thAndTodaysDateIsThe21st()
        {
            var scheduledDateTime = new DateTime(2018, 05, 14);

            var x = scheduledDateTime.AddDays(7);

            var nextDate = new ScheduledDates().GetNextScheduledDate(new DateTime(2018, 05, 20), scheduledDateTime);

            nextDate.Date.Should().Be(new DateTime(2018, 05, 21));
        }

        [TestMethod]
        public void GetNextDateForSystemSucessfullyRunsOnThe14thAndTodaysDateIsThe22nd()
        {
            var scheduledDateTime = new DateTime(2018, 05, 14);

            var nextDate = new ScheduledDates().GetNextScheduledDate(new DateTime(2018, 05, 22), scheduledDateTime);

            nextDate.Date.Should().Be(new DateTime(2018, 05, 28));
        }

        [TestMethod]
        public void GetNextDateForSystemSucessfullyRunsOnThe14thAndTodaysDateIsThe25th()
        {
            var scheduledDateTime = new DateTime(2018, 05, 14);

            var nextDate = new ScheduledDates().GetNextScheduledDate(new DateTime(2018, 05, 22), scheduledDateTime);

            nextDate.Date.Should().Be(new DateTime(2018, 05, 28));
        }

        [TestMethod]
        public void GetNextDateForSystemSucessfullyRunsOnThe14thAndTodaysDateIsThe28th()
        {
            var scheduledDateTime = new DateTime(2018, 05, 14);

            var nextDate = new ScheduledDates().GetNextScheduledDate(new DateTime(2018, 05, 22), scheduledDateTime);

            nextDate.Date.Should().Be(new DateTime(2018, 05, 28));
        }
        [TestMethod]
        public void GetNextDateForSystemSucessfullyRunsOnThe14thAndTodaysDateIsThe29th()
        {
            var scheduledDateTime = new DateTime(2018, 05, 14);

            var nextDate = new ScheduledDates().GetNextScheduledDate(new DateTime(2018, 05, 29), scheduledDateTime);

            nextDate.Date.Should().Be(new DateTime(2018, 06, 04));
        }       
    }
}
