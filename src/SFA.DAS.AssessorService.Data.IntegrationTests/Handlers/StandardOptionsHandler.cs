using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public class StandardOptionsHandler
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(StandardOptionModel standardOption)
        {
            var sqlToInsertOption =
                "INSERT INTO [dbo].[StandardOptions] " +
                    "([StandardUId], " +
                    "[OptionName]) " +
                "VALUES " +
                    "(@StandardUId, " +
                    "@optionName)";

            DatabaseService.Execute(sqlToInsertOption, standardOption);
        }

        public static void InsertRecords(List<StandardOptionModel> options)
        {
            foreach (var option in options)
            {
                InsertRecord(option);
            }
        }

        public static void DeleteAllRecords()
        {
            var sql = "DELETE FROM [StandardOptions]";

            DatabaseService.Execute(sql);
        }
    }
}
