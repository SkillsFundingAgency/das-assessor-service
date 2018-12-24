using System.Collections.Generic;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Private
{
    public class CertificateApprovalViewModel
    {     
        public IEnumerable<CertificateDetailApprovalViewModel> ApprovedCertificates { get; set; }
        public IEnumerable<CertificateDetailApprovalViewModel> RejectedCertificates { get; set; }
        public IEnumerable<CertificateDetailApprovalViewModel> ToBeApprovedCertificates { get; set; }
        public IEnumerable<CertificateDetailApprovalViewModel> SentForApprovalCertificates { get; set; }
    }
}