using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetAssessmentOrganisationByEmailRequest : IRequest<AparSummary>
    {
        public string Email { get; set; }
    }
}
