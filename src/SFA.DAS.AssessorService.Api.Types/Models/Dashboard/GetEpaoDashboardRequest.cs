using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Dashboard
{
    public class GetEpaoDashboardRequest : IRequest<GetEpaoDashboardResponse>
    {
        public string EndPointAssessorOrganisationId { get; set; }
    }
}
