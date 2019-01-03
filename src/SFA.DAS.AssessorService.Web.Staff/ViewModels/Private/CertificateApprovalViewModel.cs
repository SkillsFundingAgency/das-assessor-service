using System.Collections.Generic;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Private
{
    public class CertificateApprovalViewModel
    {     
        public PaginatedList<CertificateDetailApprovalViewModel> ApprovedCertificates { get; set; }
        public PaginatedList<CertificateDetailApprovalViewModel> RejectedCertificates { get; set; }
        public PaginatedList<CertificateDetailApprovalViewModel> ToBeApprovedCertificates { get; set; }
        public PaginatedList<CertificateDetailApprovalViewModel> SentForApprovalCertificates { get; set; }
    }
}