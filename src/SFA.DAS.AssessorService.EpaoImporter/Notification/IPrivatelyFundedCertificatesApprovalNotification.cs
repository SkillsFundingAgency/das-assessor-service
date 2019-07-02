using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;

namespace SFA.DAS.AssessorService.EpaoImporter.Notification
{
    public interface IPrivatelyFundedCertificatesApprovalNotification
    {
        Task Send(IEnumerable<CertificateResponse> certificates);
    }
}