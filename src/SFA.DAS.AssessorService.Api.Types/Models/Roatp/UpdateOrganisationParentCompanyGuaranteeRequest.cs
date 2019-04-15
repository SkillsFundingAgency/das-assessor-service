using System;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp
{
    public class UpdateOrganisationParentCompanyGuaranteeRequest: IRequest
    {
        public Guid OrganisationId { get; set; }
        public bool ParentCompanyGuarantee { get; set; }
        public string UpdatedBy { get; set; }
    }
}
