using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.DTOs;
using SFA.DAS.AssessorService.EpaoImporter.Logger;

namespace SFA.DAS.AssessorService.EpaoImporter.Data
{
    public class AssessorServiceApi : IAssessorServiceApi
    {
        private readonly IAggregateLogger _aggregateLogger;
        private readonly HttpClient _httpClient;

        public AssessorServiceApi(IAggregateLogger aggregateLogger,
            HttpClient httpClient)
        {
            _aggregateLogger = aggregateLogger;
            _httpClient = httpClient;
        }

        public async Task UpdatePrivatelyFundedCertificateRequestsToBeApproved()
        {
            var responseMessage = await _httpClient.PutAsJsonAsync(
                $"/api/v1/certificates/updatestatustobeapproved", new Object());            
        }

        public async Task<IEnumerable<CertificateResponse>> GetCertificatesToBeApproved()
        {
            var response = await _httpClient.GetAsync(
                "/api/v1/certificates?statuses=ToBeApproved");

            var certificates = await response.Content.ReadAsAsync<List<CertificateResponse>>();
            if (response.IsSuccessStatusCode)
            {
                _aggregateLogger.LogInfo($"Getting Certificates to be printed - Status code returned: {response.StatusCode}. Content: {response.Content.ReadAsStringAsync().Result}");
            }
            else
            {
                _aggregateLogger.LogInfo($"Getting Certificates to be printed - Status code returned: {response.StatusCode}. Content: {response.Content.ReadAsStringAsync().Result}");
            }

            return certificates;
        }

        public async Task<EmailTemplateSummary> GetEmailTemplate(string templateName)
        {           
            var response = await _httpClient.GetAsync(
                $"/api/v1/emailTemplates/{templateName}");

            var emailTemplate = await response.Content.ReadAsAsync<EmailTemplateSummary>();
            if (response.IsSuccessStatusCode)
            {
                _aggregateLogger.LogInfo($"Status code returned: {response.StatusCode}. Content: {response.Content.ReadAsStringAsync().Result}");
            }
            else
            {
                _aggregateLogger.LogInfo($"Status code returned: {response.StatusCode}. Content: {response.Content.ReadAsStringAsync().Result}");
            }

            return emailTemplate;
        }

        public async Task GatherStandards()
        {
            await _httpClient.PostAsJsonAsync("/api/ao/update-standards", new {});
        }
    }
}
