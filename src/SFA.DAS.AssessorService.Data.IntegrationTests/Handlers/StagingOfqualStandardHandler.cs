using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public class StagingOfqualStandardHandler
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(StagingOfqualStandardModel stagingOfqualStandard)
        {
            var sqlToInsert =
                "INSERT INTO [dbo].[StagingOfqualStandard]" +
                    "([RecognitionNumber]" +
                    ", [OperationalStartDate]" +
                    ", [OperationalEndDate]" +
                    ", [IfateReferenceNumber]) " +
                "VALUES " +
                    "(@recognitionNumber" +
                    ", @operationalStartDate" +
                    ", @operationalEndDate" +
                    ", @ifateReferenceNumber)";

            DatabaseService.Execute(sqlToInsert, stagingOfqualStandard);
        }

        public static void InsertRecords(List<StagingOfqualStandardModel> stagingOfqualStandards)
        {
            foreach (var stagingOfqualStandard in stagingOfqualStandards)
            {
                InsertRecord(stagingOfqualStandard);
            }
        }

        public static void DeleteAllRecords()
        {
            var sql = "DELETE FROM [StagingOfqualStandard]";

            DatabaseService.Execute(sql);
        }
    }
}
