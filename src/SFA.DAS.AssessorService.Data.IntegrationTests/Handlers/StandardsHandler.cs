using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public class StandardsHandler : HandlerBase
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

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
    }
}