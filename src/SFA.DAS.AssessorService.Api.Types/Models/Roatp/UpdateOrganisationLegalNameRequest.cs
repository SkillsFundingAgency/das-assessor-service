using System.Runtime.Serialization;

namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp
{
    using System;
    using System.Runtime.Serialization;
    using MediatR;

    public class UpdateOrganisationLegalNameRequest : IRequest
    {
        [DataMember]
        public Guid OrganisationId { get; set; }
        [DataMember]
        public string LegalName { get; set; }
        [DataMember]
        public string UpdatedBy { get; set; }
    }
}
