using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.EpaoImporter.Data
{
    public interface IAssessorServiceApi
    {
        Task<BatchLogResponse> CreateBatchLog(CreateBatchLogRequest createBatchLogRequest);
        Task<BatchLogResponse> GetCurrentBatchLog();
        Task<IEnumerable<CertificateResponse>> GetCertificatesToBePrinted();
        Task ChangeStatusToPrinted(int batchNumber, IEnumerable<CertificateResponse> responses);
        Task<EMailTemplate> GetEmailTemplate();
        Task<ScheduleRun> GetSchedule(ScheduleType scheduleType);
    }
}