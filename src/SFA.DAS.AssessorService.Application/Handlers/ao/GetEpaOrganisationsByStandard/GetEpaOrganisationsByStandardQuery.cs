using MediatR;

namespace SFA.DAS.AssessorService.Application.Handlers.ao.GetEpaOrganisationsByStandard
{
    public class GetEpaOrganisationsByStandardQuery : IRequest<GetEpaOrganisationsByStandardResponse>
    {
        public int Standard { get; set; }
    }
}
