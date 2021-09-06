using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class OrganisationStandardVersionOptInRequest : IRequest<OrganisationStandardVersion>
    {
        public Guid ApplicationId { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public string StandardReference { get; set; }
        public string Version { get; set; }
        public string StandardUId { get; set; }

        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? DateVersionApproved { get; set; }

        public string Comments { get; set; }

        public string Status { get; set; }

        public Guid SubmittingContactId { get; set; }
    }

}
