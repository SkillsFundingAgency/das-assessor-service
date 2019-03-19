using System.Collections.Generic;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public class StandardCollationHandler
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(StandardCollationModel standardCollation)
        {
            var sqlToInsert = @"INSERT INTO [dbo].[StandardCollation]
                    ([StandardId]
                ,[ReferenceNumber]
                ,[Title]
                ,[StandardData])
            VALUES
                (@standardId
                ,@referenceNumber
                ,@Title
                ,@standardData);";
            DatabaseService.Execute(sqlToInsert, standardCollation);
        }

        public static void InsertRecords(List<StandardCollationModel> standardCollations)
        {
            foreach (var standardCollation in standardCollations)
            {
                InsertRecord(standardCollation);
            }
        }

        public static void DeleteRecord(int idToDelete)
        {
            var sql = $@"DELETE from StandardCollation where id = {idToDelete}; ";
            DatabaseService.Execute(sql);
        }

        public static void DeleteAllRecords()
        {
            var sql = $@"DELETE from StandardCollation";
            DatabaseService.Execute(sql);
        }

        public static void DeleteRecords(List<int> ids)
        {
            foreach (var id in ids)
            {
                DeleteRecord(id);
            }
        }
    }
}
