using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Private
{
    public class CertificateApprovalViewModel
    {
        public int Id { get; set; }
        public IEnumerable<CertificateDetailApprovalViewModel> CertificateDetailApprovalViewModels { get; set; }
    }
}