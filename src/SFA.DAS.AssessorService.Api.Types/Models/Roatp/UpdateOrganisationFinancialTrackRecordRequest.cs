
namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp
{
    using MediatR;
    using System;
    using System.Runtime.Serialization;

    public class UpdateOrganisationFinancialTrackRecordRequest : IRequest
    {
        [DataMember]
        public Guid OrganisationId { get; set; }
        [DataMember]
        public bool FinancialTrackRecord { get; set; }
        [DataMember]
        public string UpdatedBy { get; set; }
    }
}
