using System.Runtime.Serialization;
using System;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp
{
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
