using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.EpaoImporter.Const;
using SFA.DAS.AssessorService.EpaoImporter.Extensions;
using SFA.DAS.AssessorService.EpaoImporter.InfrastructureServices;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.EpaoImporter.Data
{
    public class AssessorServiceApi : IAssessorServiceApi
    {
        private readonly IAggregateLogger _aggregateLogger;
        private readonly HttpClient _httpClient;
        private readonly IWebConfiguration _webConfiguration;
        private readonly TokenService _tokenService;

        public AssessorServiceApi(IAggregateLogger aggregateLogger,
            HttpClient httpClient,
            IWebConfiguration webConfiguration,
            TokenService tokenService)
        {
            _aggregateLogger = aggregateLogger;
            _httpClient = httpClient;
            _webConfiguration = webConfiguration;
            _tokenService = tokenService;
        }

        public async Task<IEnumerable<CertificateResponse>> GetCertificatesToBePrinted()
        {
            var response = await _httpClient.GetAsync(
                "/api/v1/certificates?status=Submitted");

            var certificates = response.Deserialise<List<CertificateResponse>>();
            if (response.IsSuccessStatusCode)
            {
                _aggregateLogger.LogInfo($"Status code returned: {response.StatusCode}. Content: {response.Content.ReadAsStringAsync().Result}");
            }
            else
            {
                _aggregateLogger.LogInfo($"Status code returned: {response.StatusCode}. Content: {response.Content.ReadAsStringAsync().Result}");
            }

            return certificates;
        }

        public async Task<int> GenerateBatchNumber()
        {
            var response = await _httpClient.GetAsync(
                "/api/v1/certificates/generatebatchnumber");

            return response.Deserialise<int>();
        }

        public async Task ChangeStatusToPrinted(int batchNumber, IEnumerable<CertificateResponse> responses)
        {
            var certificateStatuses = responses.Select(
                q => new CertificateStatus
                {
                    CertificateReference = q.CertificateReference,
                    Status = Domain.Consts.CertificateStatus.Printed
                }).ToList();

            var updateCertificatesBatchToIndicatePrintedRequest = new UpdateCertificatesBatchToIndicatePrintedRequest
            {
                BatchNumber = batchNumber,
                CertificateStatuses = certificateStatuses
            };

            var jsonData = JsonConvert.SerializeObject(updateCertificatesBatchToIndicatePrintedRequest);

            var responseMessage = await _httpClient.PutAsJsonAsync(
                "/api/v1/certificates/{batchNumber}", updateCertificatesBatchToIndicatePrintedRequest);
        }

        public async Task<EMailTemplate> GetEmailTemplate()
        {
            var templateName = EMailTemplateNames.PrintAssessorCoverLetters;
            var response = await _httpClient.GetAsync(
                $"/api/v1/emailTemplates/{templateName}");

            var emailTemplate = response.Deserialise<EMailTemplate>();
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
    }
}
