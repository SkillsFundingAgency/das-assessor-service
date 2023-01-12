
namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp
{
    using MediatR;
    using System;
    using System.Runtime.Serialization;

    public class UpdateOrganisationParentCompanyGuaranteeRequest : IRequest
    {
        [DataMember]
        public Guid OrganisationId { get; set; }
        [DataMember]
        public bool ParentCompanyGuarantee { get; set; }
        [DataMember]
        public string UpdatedBy { get; set; }
    }
}
