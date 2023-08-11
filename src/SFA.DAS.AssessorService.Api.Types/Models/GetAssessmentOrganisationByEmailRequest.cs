using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetAssessmentOrganisationByEmailRequest : IRequest<AssessmentOrganisationSummary>
    {
        public string Email { get; set; }
    }
}
