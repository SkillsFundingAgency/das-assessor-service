using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;

namespace SFA.DAS.AssessorService.EpaoImporter.Notification
{
    public interface INotificationService
    {
        Task Send(int batchNumber, List<CertificateResponse> certificates, int coverLettersProduced);
    }
}