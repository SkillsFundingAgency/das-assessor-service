using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Register
{
    public class UpdateEpaOrganisationStandardVersionValidationRequest : IRequest<ValidationResponse>
    {
        public int OrganisationStandardId { get; set; }
        public string OrganisationStandardVersion { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }
}
