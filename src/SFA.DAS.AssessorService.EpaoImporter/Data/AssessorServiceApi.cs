using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.EpaoImporter.Const;
using SFA.DAS.AssessorService.EpaoImporter.Extensions;
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

        public async Task<BatchLogResponse> CreateBatchLog(CreateBatchLogRequest createBatchLogRequest)
        {
            var responseMessage = await _httpClient.PostAsJsonAsync(
                $"/api/v1/batches", createBatchLogRequest);

            return responseMessage.Deserialise<BatchLogResponse>();
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

        public async Task<BatchLogResponse> GetCurrentBatchLog()
        {
            var response = await _httpClient.GetAsync(
                "/api/v1/batches/latest");

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return new BatchLogResponse
                {
                    BatchNumber = 0,
                    ScheduledDate = DateTime.Now.GetNextWeekday(DayOfWeek.Monday).AddDays(-7)                    
                };
            }

            return response.Deserialise<BatchLogResponse>();
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

            var responseMessage = await _httpClient.PutAsJsonAsync(
                $"/api/v1/certificates/{batchNumber.ToString()}", updateCertificatesBatchToIndicatePrintedRequest);
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
