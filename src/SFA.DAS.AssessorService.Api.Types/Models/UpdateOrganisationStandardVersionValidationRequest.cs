using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class UpdateOrganisationStandardVersionValidationRequest : IRequest<ValidationResponse>
    {
        public int OrganisationStandardId { get; set; }
        public string OrganisationStandardVersion { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }
}
