using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public class StandardsHandler : HandlerBase
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static StandardModel Create(string title, string referenceNumber, int larsCode, string version, 
            DateTime? effectiveFrom, DateTime? effectiveTo, DateTime? versionEarliestStartDate, DateTime? versionLatestStartDate, DateTime? versionLatestEndDate,
            DateTime? versionApprovedForDelivery, bool epaChanged, string eqaProviderName, bool epaoMustBeApprovedByRegulatorBody)
        {
            ConvertVersionStringToInts(version, out int major, out int minor);
         
            return new StandardModel
            {
                StandardUId = $"{referenceNumber}_{version}",
                IFateReferenceNumber = referenceNumber,
                LarsCode = larsCode,
                Title = title,
                Version = version,
                Level = 4,
                Status = "Approved for delivery",
                EffectiveFrom = effectiveFrom,
                EffectiveTo = effectiveTo,
                VersionEarliestStartDate = versionEarliestStartDate,
                VersionLatestStartDate = versionLatestStartDate,
                VersionLatestEndDate = versionLatestEndDate,
                TypicalDuration = 12,
                VersionApprovedForDelivery = versionApprovedForDelivery,
                EPAChanged = epaChanged,
                TrailblazerContact = "TrailblazerContact",
                VersionMajor = major,
                VersionMinor = minor,
                StandardPageUrl = "www.standard.com",
                EqaProviderName = eqaProviderName,
                OverviewOfRole = "OverviewOfRole",
                EpaoMustBeApprovedByRegulatorBody = epaoMustBeApprovedByRegulatorBody
            };
        }

        public static void InsertRecord(StandardModel standard)
        {
            var sqlToInsertStandard =
                "INSERT INTO [dbo].[Standards]" +
                    "([StandardUId]" +
                    ", [IFateReferenceNumber]" +
                    ", [LarsCode]" +
                    ", [Title]" +
                    ", [Version]" +
                    ", [Level]" +
                    ", [Status]" +
                    ", [TypicalDuration]" +
                    ", [MaxFunding]" +
                    ", [IsActive]" +
                    ", [EffectiveFrom]" +
                    ", [EffectiveTo]" +
                    ", [VersionEarliestStartDate]" +
                    ", [VersionLatestStartDate]" +
                    ", [VersionLatestEndDate]" +
                    ", [ProposedTypicalDuration]" +
                    ", [ProposedMaxFunding]" +
                    ", [EPAChanged]" +
                    ", [StandardPageUrl]" +
                    ", [TrailblazerContact]" +
                    ", [VersionMajor]" +
                    ", [VersionMinor]" +
                    ", [EqaProviderName]" +
                    ", [OverviewOfRole]" +
                    ", [VersionApprovedForDelivery]" +
                    ", [EpaoMustBeApprovedByRegulatorBody])" +
                "VALUES " +
                    "(@StandardUId" +
                    ", @iFateReferenceNumber" +
                    ", @larsCode" +
                    ", @title" +
                    ", @version" +
                    ", @level" +
                    ", @status" +
                    ", @typicalDuration" +
                    ", @maxFunding" +
                    ", @isActive" +
                    ", @effectiveFrom" +
                    ", @effectiveTo" +
                    ", @versionEarliestStartDate" +
                    ", @versionLatestStartDate" +
                    ", @versionLatestEndDate" +
                    ", @proposedTypicalDuration" +
                    ", @proposedMaxFunding" +
                    ", @epaChanged" +
                    ", @standardPageUrl" +
                    ", @trailblazerContact" +
                    ", @versionMajor" +
                    ", @versionMinor" +
                    ", @eqaProviderName" +
                    ", @overviewOfRole" +
                    ", @versionApprovedForDelivery" +
                    ", @epaoMustBeApprovedByRegulatorBody)";

            DatabaseService.Execute(sqlToInsertStandard, standard);
        }

        public static void InsertRecords(List<StandardModel> standards)
        {
            foreach (var standard in standards)
            {
                InsertRecord(standard);
            }
        }

        public static void DeleteAllRecords()
        {
            var sql = "DELETE FROM [Standards]";

            DatabaseService.Execute(sql);
        }

        private static void ConvertVersionStringToInts(string version, out int major, out int minor)
        {
            string[] parts = version.Split('.');
            if (parts.Length != 2)
            {
                throw new ArgumentException("Invalid version format");
            }

            major = int.Parse(parts[0]);
            minor = int.Parse(parts[1]);
        }
    }
}