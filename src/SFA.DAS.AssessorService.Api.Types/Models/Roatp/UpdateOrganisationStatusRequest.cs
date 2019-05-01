namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp
{
    using System;
    using System.Runtime.Serialization;
    using MediatR;

    public class UpdateOrganisationStatusRequest : IRequest
    {
        [DataMember]
        public Guid OrganisationId { get; set; }
        [DataMember]
        public int OrganisationStatusId { get; set; }
        [DataMember]
        public int? RemovedReasonId { get; set; }
        [DataMember]
        public string UpdatedBy { get; set; }
    }
}
