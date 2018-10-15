using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Private
{
    public class CertificateApprovalViewModel
    {     
        public IEnumerable<CertificateDetailApprovalViewModel> ApprovedCertificates { get; set; }
        public IEnumerable<CertificateDetailApprovalViewModel> RejectedCertificates { get; set; }
        public IEnumerable<CertificateDetailApprovalViewModel> ToBeApprovedCertificates { get; set; }
    }
}