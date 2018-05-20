using System;
using SFA.DAS.AssessorService.EpaoImporter.Interfaces;
using SFA.DAS.AssessorService.EpaoImporter.Logger;

namespace SFA.DAS.AssessorService.EpaoImporter.Helpers
{
    public class DateTimeZoneInformation : IDateTimeZoneInformation
    {
        private readonly IAggregateLogger _aggregateLogger;

        public DateTimeZoneInformation(IAggregateLogger aggregateLogger)
        {
            _aggregateLogger = aggregateLogger;
        }

        public void GetCurrentTimeZone()
        {
            _aggregateLogger.LogInfo("Getting Current Timezone");
            var currentTimeZone = TimeZone.CurrentTimeZone;
            var localTime = DateTime.Now;
            _aggregateLogger.LogInfo($"Current Timezone Standard Name = {currentTimeZone.StandardName}");
            _aggregateLogger.LogInfo($"Current Timezone Daylight = {currentTimeZone.DaylightName}");
            _aggregateLogger.LogInfo($"Current Timezone is Daylight Savings Time = {currentTimeZone.IsDaylightSavingTime(localTime)}");
            _aggregateLogger.LogInfo($"LocalTime = {localTime}");
            _aggregateLogger.LogInfo($"Utc Offset = {currentTimeZone.GetUtcOffset(localTime)}");
            _aggregateLogger.LogInfo($"Utc Time = {localTime.ToUniversalTime()}");
        }

        public void DisplayTimeZoneNames()
        {
            foreach (TimeZoneInfo z in TimeZoneInfo.GetSystemTimeZones())
                _aggregateLogger.LogInfo(z.Id);
        }
    }
}
