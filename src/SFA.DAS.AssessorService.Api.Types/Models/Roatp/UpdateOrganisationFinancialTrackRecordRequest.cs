
namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp
{
    using System;
    using MediatR;
    using System.Runtime.Serialization;

    public class UpdateOrganisationFinancialTrackRecordRequest: IRequest
    {
        [DataMember]
        public Guid OrganisationId { get; set; }
        [DataMember]
        public bool FinancialTrackRecord { get; set; }
        [DataMember]
        public string UpdatedBy { get; set; }
    }
}
