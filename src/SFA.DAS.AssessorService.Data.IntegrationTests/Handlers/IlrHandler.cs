using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public class IlrHandler
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(IlrModel ilrModel)
        {
            var sql = @"INSERT INTO [dbo].[Ilrs]
                            ([Id],
                                [Source],
                                [Ukprn],
                                [ULN],
                                [StdCode],
                                [FundingModel],
                                [GivenNames],
                                [FamilyName],
                                [EpaOrgId],
                                [LearnStartDate],
                                [PlannedEndDate],
                                [CompletionStatus],
                                [LearnRefNumber],
                                [DelLocPostCode],
                                [LearnActEndDate],
                                [WithdrawReason],
                                [Outcome],
                                [AchDate],
                                [OutGrade],
                                [CreatedAt],
                                [UpdatedAt])
                            VALUES
                                (@id,
                                @source,
                                @ukprn,
                                @uLN,
                                @stdCode,
                                @fundingModel,
                                @givenNames,
                                @familyName,
                                @epaOrgId,
                                @learnStartDate,
                                @plannedEndDate,
                                @completionStatus,
                                @learnRefNumber,
                                @delLocPostCode,
                                @learnActEndDate,
                                @withdrawReason,
                                @outcome,
                                @achDate,
                                @outGrade,
                                @createdAt,
                                @updatedAt)";

            DatabaseService.Execute(sql, ilrModel);
        }
    }
}
