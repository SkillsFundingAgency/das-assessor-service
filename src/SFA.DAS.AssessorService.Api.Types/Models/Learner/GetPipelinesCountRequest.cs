using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetPipelinesCountRequest : IRequest<int>
    {
        public GetPipelinesCountRequest(string endpointAssessmentOrganisationId, int? standardCode)
        {
            EndpointAssessmentOrganisationId = endpointAssessmentOrganisationId;
            StandardCode = standardCode;
        }

        public string EndpointAssessmentOrganisationId { get; }
        public int? StandardCode { get; }
    }
}