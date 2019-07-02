using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData.Printing;

namespace SFA.DAS.AssessorService.EpaoImporter.Data
{
    public interface IAssessorServiceApi
    {
        Task<BatchLogResponse> CreateBatchLog(CreateBatchLogRequest createBatchLogRequest);
        Task<BatchLogResponse> GetCurrentBatchLog();
        Task<BatchLogResponse> GetGetBatchLogByBatchNumber(string batchNumber);
        Task UpdateBatchDataInBatchLog(Guid batchId, BatchData batchData);
        Task<IEnumerable<CertificateResponse>> GetCertificatesToBePrinted();
        Task ChangeStatusToPrinted(IEnumerable<CertificateResponse> responses);
        Task<EMailTemplate> GetEmailTemplate(string templateName);
        Task<ScheduleRun> GetSchedule(ScheduleType scheduleType);
        Task CompleteSchedule(Guid scheduleRunId);
        Task UpdateBatchNumberInCertificates(int batchNumber, IEnumerable<CertificateResponse> responses);

        Task GatherStandards();
    }
}