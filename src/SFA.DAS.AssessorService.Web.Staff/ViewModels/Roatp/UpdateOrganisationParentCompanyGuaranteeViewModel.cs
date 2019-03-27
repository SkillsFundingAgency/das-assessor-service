using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp
{
    public class UpdateOrganisationParentCompanyGuaranteeViewModel
    {
        public bool CurrentParentCompanyGuarantee { get; set; }
        public Guid OrganisationId { get; set; }
        public bool ParentCompanyGuarantee { get; set; }
        public string UpdatedBy { get; set; }
        public string LegalName { get; set; }
    }
}
