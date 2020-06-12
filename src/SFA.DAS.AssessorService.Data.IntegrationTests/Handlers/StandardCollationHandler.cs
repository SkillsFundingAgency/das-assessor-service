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
            var sqlToInsertStandardCollation = 
                "INSERT INTO [dbo].[StandardCollation]" + 
                    "([StandardId]" + 
                    ",[ReferenceNumber]" + 
                    ",[Title]" + 
                    ",[StandardData]" + 
                    ",[IsLIve])" + 
                "VALUES" + 
                    "(@standardId" + 
                    ",@referenceNumber" + 
                    ",@Title" + 
                    ",@standardData" +
                    ",@isLive);";

            DatabaseService.Execute(sqlToInsertStandardCollation, standardCollation);

            var sqlToInsertOption =
                "INSERT INTO [dbo].[Options]" +
                    "([StdCode]" +
                    ",[OptionName]" +
                    ",[IsLive])" +
                "VALUES" +
                    "(@stdCode" +
                    ",@optionName" +
                    ",@isLive);";

            foreach (var optionDataModel in standardCollation.Options)
            {
                DatabaseService.Execute(sqlToInsertOption, optionDataModel);
            }
        }

        public static void InsertRecords(List<StandardCollationModel> standardCollations)
        {
            foreach (var standardCollation in standardCollations)
            {
                InsertRecord(standardCollation);
            }
        }

        public static void DeleteAllRecords()
        {
            foreach (string sql in new List<string>
            {
                "DELETE FROM [StandardCollation]",
                "DELETE FROM [Options]"
            })
            {
                DatabaseService.Execute(sql);
            }
        }
    }
}
