using System.Collections.Generic;
using System.Linq;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public static class StagingStandardOptionsHandler
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(StandardOptionModel option)
        {
            DatabaseService.Execute(@"
                INSERT INTO [dbo].[StagingStandardOptions] ([StandardUId], [OptionName])
                VALUES (@StandardUId, @OptionName);", option);
        }

        public static List<StandardOptionModel> GetRecords()
        {
            return DatabaseService.GetList<StandardOptionModel>(
                "SELECT [StandardUId], [OptionName] FROM [dbo].[StagingStandardOptions]").ToList();
        }

        public static void DeleteAllRecords()
        {
            DatabaseService.Execute("DELETE FROM [dbo].[StagingStandardOptions]");
        }
    }
}
