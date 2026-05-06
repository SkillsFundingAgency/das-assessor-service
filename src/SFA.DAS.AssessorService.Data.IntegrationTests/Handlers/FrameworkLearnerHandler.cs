using System;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public class FrameworkLearnerHandler : HandlerBase
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(FrameworkLearnerModel frameworkLearner)
        {
            var sql =
                "INSERT INTO [dbo].[FrameworkLearner] " +
                "([Id], [FrameworkCertificateNumber], [CertificationYear], [CertificationDate], [ApprenticeFullname], " +
                "[ApprenticeSurname], [ApprenticeForename], [ApprenticeDoB], [ApprenticeULN], [TrainingCode], " +
                "[FrameworkName], [PathwayName], [ApprenticeshipLevel], [ProviderName], [Ukprn], [Framework], " +
                "[Pathway], [ApprenticeshipLevelName], [ApprenticeId], [CreatedOn], [ApprenticeNameMatch]) " +
                "VALUES " +
                "(@Id, @FrameworkCertificateNumber, @CertificationYear, @CertificationDate, @ApprenticeFullname, " +
                "@ApprenticeSurname, @ApprenticeForename, @ApprenticeDoB, @ApprenticeULN, @TrainingCode, " +
                "@FrameworkName, @PathwayName, @ApprenticeshipLevel, @ProviderName, @Ukprn, @Framework, " +
                "@Pathway, @ApprenticeshipLevelName, @ApprenticeId, @CreatedOn, @ApprenticeNameMatch)";

            DatabaseService.Execute(sql, frameworkLearner);
        }

        public static void DeleteAllRecords()
        {
            var sql = "DELETE FROM [dbo].[FrameworkLearner]";
            DatabaseService.Execute(sql);
        }
    }
}
