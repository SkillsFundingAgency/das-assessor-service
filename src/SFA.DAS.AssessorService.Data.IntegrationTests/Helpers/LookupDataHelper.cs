using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System.IO;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.IntegrationTests
{
    public class LookupDataHelper
    {
        private readonly static DatabaseService _databaseService = new DatabaseService();
        
        public async static void AddLookupData()
        {
            var content = await ReadFileAsync("LookupData\\OrganisationTypeInsertOrUpdate.sql");
            _databaseService.Execute(content);
        }

        public static async Task<string> ReadFileAsync(string fileName)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(currentDirectory, fileName);

            using (var reader = new StreamReader(filePath))
            {
                var content = await reader.ReadToEndAsync();
                return content;
            }
        }
    }
}

