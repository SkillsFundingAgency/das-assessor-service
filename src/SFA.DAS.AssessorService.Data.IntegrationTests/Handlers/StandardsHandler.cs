using System.Collections.Generic;
using System.Linq;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public class StandardsHandler : HandlerBase
    {
        private static readonly DatabaseService DatabaseService =
            new DatabaseService();

        public static void InsertRecord(StandardModel standard)
        {
            const string sql = @"
                INSERT INTO [dbo].[Standards]
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
                    [EpaoMustBeApprovedByRegulatorBody],
                    [UpdatedAt]
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
                    @EpaoMustBeApprovedByRegulatorBody,
                    @UpdatedAt
                );";

            DatabaseService.Execute(sql, standard);
        }

        public static void InsertRecords(List<StandardModel> standards)
        {
            foreach (var standard in standards)
            {
                InsertRecord(standard);
            }
        }

        public static List<StandardModel> GetRecords()
        {
            return DatabaseService.GetList<StandardModel>(
                "SELECT * FROM [dbo].[Standards]").ToList();
        }

        public static void DeleteAllRecords()
        {
            DatabaseService.Execute("DELETE FROM [dbo].[Standards]");
        }
    }
}