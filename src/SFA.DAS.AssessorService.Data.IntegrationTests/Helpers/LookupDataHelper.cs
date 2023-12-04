using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System.IO;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.IntegrationTests
{
    public class LookupDataHelper
    {
        private readonly static DatabaseService _databaseService = new DatabaseService();
        
        public async static Task AddLookupData()
        {
            await AddLookupDataFile("LookupData\\DeliveryAreaInsertOrUpdate.sql");
            await AddLookupDataFile("LookupData\\EmailTemplatesInsertOrUpdate.sql");
            await AddLookupDataFile("LookupData\\OrganisationTypeInsertOrUpdate.sql");
            await AddLookupDataFile("LookupData\\PostCodeRegionInsertOrUpdate.sql");
            await AddLookupDataFile("LookupData\\PrivilegesInsertOrUpdate.sql");
            await AddLookupDataFile("LookupData\\StaffReportsInsertOrUpdate.sql");
        }

        private static async Task AddLookupDataFile(string fileName)
        {
            var content = await ReadFileAsync(fileName);
            _databaseService.Execute(content);
        }

        private static async Task<string> ReadFileAsync(string fileName)
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

