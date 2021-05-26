using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public class StandardsHandler
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(StandardModel standard)
        {
            var sqlToInsertStandard =
                "INSERT INTO [dbo].[Standards]" +
                    "([StandardUId]" +
                    ", [IFateReferenceNumber]" +
                    ", [LarsCode]" +
                    ", [Version]" +
                    ", [Title]" +
                    ", [Level]" +
                    ", [Status]" +
                    ", [TypicalDuration]" +
                    ", [MaxFunding]" +
                    ", [IsActive]" +
                    ", [ProposedTypicalDuration]" +
                    ", [ProposedMaxFunding]" +
                    ", [EPAChanged])" +
                "VALUES " +
                    "(@StandardUId" +
                    ", @iFateReferenceNumber" +
                    ", @larsCode" +
                    ", @version" +
                    ", @title" +
                    ", @level" +
                    ", @status" +
                    ", @typicalDuration" +
                    ", @maxFunding" +
                    ", @isActive" +
                    ", @proposedTypicalDuration" +
                    ", @proposedMaxFunding" +
                    ", @epaChanged)";

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
