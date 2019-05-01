using System;
using System.Runtime.Serialization;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp
{
    public class UpdateOrganisationUkprnRequest : IRequest
    {
        [DataMember]
        public Guid OrganisationId { get; set; }
        [DataMember]
        public string Ukprn { get; set; }
        [DataMember]
        public string UpdatedBy { get; set; }
    }
}
