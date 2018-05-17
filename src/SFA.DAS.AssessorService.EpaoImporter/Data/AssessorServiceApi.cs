using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentDateTime;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.EpaoImporter.Const;
using SFA.DAS.AssessorService.EpaoImporter.Extensions;
using SFA.DAS.AssessorService.EpaoImporter.Helpers;
using SFA.DAS.AssessorService.EpaoImporter.Logger;

namespace SFA.DAS.AssessorService.EpaoImporter.Data
{
    public class AssessorServiceApi : IAssessorServiceApi
    {
        private readonly IAggregateLogger _aggregateLogger;
        private readonly HttpClient _httpClient;
        private readonly ScheduleConfig _scheduleConfig;

        public AssessorServiceApi(IAggregateLogger aggregateLogger,
            HttpClient httpClient,
            ScheduleConfig scheduleConfig)
        {
            _aggregateLogger = aggregateLogger;
            _httpClient = httpClient;
            _scheduleConfig = scheduleConfig;
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
                DateTime nextDate;
                if (DateTime.Now.DayOfWeek == (DayOfWeek) _scheduleConfig.DayOfWeek)
                {
                    nextDate = DateTime.Now.AddDays(-7).Date;
                }
                else
                {
                    nextDate = DateTime.Now.Previous(ScheduledDates.GetDayOfWeek(_scheduleConfig.DayOfWeek))
                        .Date;
                }

                var hourDate = nextDate.AddHours(_scheduleConfig.Hour);
                var scheduledDate = hourDate.AddMinutes(_scheduleConfig.Minute);

                return new BatchLogResponse
                {
                    BatchNumber = 0,
                    ScheduledDate = scheduledDate
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
