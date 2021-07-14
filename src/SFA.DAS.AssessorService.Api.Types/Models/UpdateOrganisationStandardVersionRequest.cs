using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class UpdateOrganisationStandardVersionRequest : IRequest<OrganisationStandardVersion>
    {
        public int OrganisationStandardId { get; set; }
        public decimal OrganisationStandardVersion { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }
}
