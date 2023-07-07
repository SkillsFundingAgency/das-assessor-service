using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class OrganisationStandardVersionOptOutRequest : IRequest<OrganisationStandardVersion>
    {
        public string EndPointAssessorOrganisationId { get; set; }
        
        public string StandardReference { get; set; }
        
        public string Version { get; set; }

        public DateTime? EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public Guid ContactId { get; set; }

        public DateTime OptOutRequestedAt { get; set; }
    }

}
