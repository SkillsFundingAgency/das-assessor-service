using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateApprovalViewModel
    {     
        public IEnumerable<CertificateDetailApprovalViewModel> DraftCertificates { get; set; }     
    }
}