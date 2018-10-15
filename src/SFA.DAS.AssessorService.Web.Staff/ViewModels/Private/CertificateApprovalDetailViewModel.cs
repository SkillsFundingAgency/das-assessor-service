using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Private
{
    public class CertificateDetailApprovalViewModel
    {
        public string FullName { get; set; }
        public string Uln { get; set; }    
        public string StandardCode { get; set; }
        public string CertificateReference { get; set; }
          
        public string IsApproved { get; set; }        
        public IEnumerable<SelectListItem> ApprovedRejected { get; set; }
    }
}