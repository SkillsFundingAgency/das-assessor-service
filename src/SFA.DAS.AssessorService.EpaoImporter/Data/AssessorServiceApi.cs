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
            return await responseMessage.Content.ReadAsAsync<BatchLogResponse>();
        }

        public async Task<IEnumerable<CertificateResponse>> GetCertificatesToBePrinted()
        {
            var response = await _httpClient.GetAsync(
                "/api/v1/certificates?statuses=Submitted&statuses=Reprint");
         
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

        public async Task<BatchLogResponse> GetCurrentBatchLog()
        {
            var response = await _httpClient.GetAsync(
                "/api/v1/batches/latest");

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return new BatchLogResponse
                {
                    BatchNumber = 0
                };
            }

            return await response.Content.ReadAsAsync<BatchLogResponse>();
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

            await _httpClient.PutAsJsonAsync($"/api/v1/certificates/{batchNumber}", updateCertificatesBatchToIndicatePrintedRequest);
        }

        public async Task<EMailTemplate> GetEmailTemplate()
        {
            var templateName = EMailTemplateNames.PrintAssessorCoverLetters;
            var response = await _httpClient.GetAsync(
                $"/api/v1/emailTemplates/{templateName}");

            var emailTemplate = await response.Content.ReadAsAsync<EMailTemplate>();
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

        public async Task<ScheduleRun> GetSchedule(ScheduleType scheduleType)
        {
            var response = await _httpClient.GetAsync($"/api/v1/schedule/runnow?scheduleType={(int) scheduleType}");
            if (!response.IsSuccessStatusCode) return null;

            var schedule = await response.Content.ReadAsAsync<ScheduleRun>();
            return schedule;

        }

        public async Task CompleteSchedule(Guid scheduleRunId)
        {
            await _httpClient.PostAsync($"/api/v1/schedule?scheduleRunId={scheduleRunId}", null);
        }
    }
}
