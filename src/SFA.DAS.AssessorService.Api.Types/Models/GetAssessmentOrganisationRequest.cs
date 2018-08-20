    using MediatR;
    using SFA.DAS.AssessorService.Api.Types.Models.AO;

    namespace SFA.DAS.AssessorService.Api.Types.Models
    {
        public class GetAssessmentOrganisationRequest: IRequest<EpaOrganisation>
        {
        public string OrganisationId { get; set; }
        }
    }

