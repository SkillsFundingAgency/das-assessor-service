using System.Collections.Generic;
using System.Linq;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public static class StagingStandardsHandler
    {
        private static readonly DatabaseService DatabaseService =
            new DatabaseService();

        public static void InsertRecord(StandardModel standard)
        {
            const string sql = @"
                INSERT INTO [dbo].[StagingStandards]
                (
                    [StandardUId],
                    [IFateReferenceNumber],
                    [LarsCode],
                    [Title],
                    [Version],
                    [Level],
                    [Status],
                    [TypicalDuration],
                    [MaxFunding],
                    [IsActive],
                    [LastDateStarts],
                    [EffectiveFrom],
                    [EffectiveTo],
                    [VersionEarliestStartDate],
                    [VersionLatestStartDate],
                    [VersionLatestEndDate],
                    [VersionApprovedForDelivery],
                    [ProposedTypicalDuration],
                    [ProposedMaxFunding],
                    [EPAChanged],
                    [StandardPageUrl],
                    [TrailBlazerContact],
                    [Route],
                    [VersionMajor],
                    [VersionMinor],
                    [IntegratedDegree],
                    [EqaProviderName],
                    [EqaProviderContactName],
                    [EqaProviderContactEmail],
                    [OverviewOfRole],
                    [CoronationEmblem],
                    [EpaoMustBeApprovedByRegulatorBody]
                )
                VALUES
                (
                    @StandardUId,
                    @IFateReferenceNumber,
                    @LarsCode,
                    @Title,
                    @Version,
                    @Level,
                    @Status,
                    @TypicalDuration,
                    @MaxFunding,
                    @IsActive,
                    @LastDateStarts,
                    @EffectiveFrom,
                    @EffectiveTo,
                    @VersionEarliestStartDate,
                    @VersionLatestStartDate,
                    @VersionLatestEndDate,
                    @VersionApprovedForDelivery,
                    @ProposedTypicalDuration,
                    @ProposedMaxFunding,
                    @EPAChanged,
                    @StandardPageUrl,
                    @TrailblazerContact,
                    @Route,
                    @VersionMajor,
                    @VersionMinor,
                    @IntegratedDegree,
                    @EqaProviderName,
                    @EqaProviderContactName,
                    @EqaProviderContactEmail,
                    @OverviewOfRole,
                    @CoronationEmblem,
                    @EpaoMustBeApprovedByRegulatorBody
                );";

            DatabaseService.Execute(sql, standard);
        }

        public static List<StandardModel> GetRecords()
        {
            return DatabaseService.GetList<StandardModel>(
                "SELECT * FROM [dbo].[StagingStandards]").ToList();
        }

        public static void DeleteAllRecords()
        {
            DatabaseService.Execute("DELETE FROM [dbo].[StagingStandards]");
        }
    }
}