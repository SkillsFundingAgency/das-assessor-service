using System.Net.Http;

namespace SFA.DAS.AssessorService.EpaoImporter.DomainServices
{
    public class UpdateStatusService
    {
        private readonly HttpClient _httpClient;

        public UpdateStatusService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

      
    }
}