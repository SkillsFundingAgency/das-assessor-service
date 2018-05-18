using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.EpaoImporter.Extensions;
using SFA.DAS.AssessorService.EpaoImporter.Logger;

namespace SFA.DAS.AssessorService.EpaoImporter.Data
{
    public class SchedulingConfigurationService : ISchedulingConfigurationService
    {
        private readonly IAggregateLogger _aggregateLogger;
        private readonly HttpClient _httpClient;

        public SchedulingConfigurationService(IAggregateLogger aggregateLogger,
            HttpClient httpClient)
        {
            _aggregateLogger = aggregateLogger;
            _httpClient = httpClient;
        }

        public async Task<ScheduleConfiguration> GetSchedulingConfiguration()
        {
            var response = await _httpClient.GetAsync(
                "/api/v1/schedulingconfiguration");

            var schedulingConfiguration = response.Deserialise<ScheduleConfiguration>();
            if (response.IsSuccessStatusCode)
            {
                _aggregateLogger.LogInfo($"Getting Scheduling Configuration - Status code returned: {response.StatusCode}. Content: {response.Content.ReadAsStringAsync().Result}");
            }
            else
            {
                _aggregateLogger.LogInfo($"Getting Scheduling Configuration - Status code returned: {response.StatusCode}. Content: {response.Content.ReadAsStringAsync().Result}");
            }

            return schedulingConfiguration;
        }
    }
}
