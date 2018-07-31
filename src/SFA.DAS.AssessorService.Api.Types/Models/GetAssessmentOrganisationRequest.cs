    using MediatR;
    using SFA.DAS.AssessorService.Api.Types.Models.AO;

    namespace SFA.DAS.AssessorService.Api.Types.Models
    {
        public class GetAssessmentOrganisationRequest: IRequest<AssessmentOrganisationDetails>
        {
        public string OrganisationId { get; set; }
        }
    }

