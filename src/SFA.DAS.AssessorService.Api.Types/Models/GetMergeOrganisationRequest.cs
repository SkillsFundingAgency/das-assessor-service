using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetMergeOrganisationRequest : IRequest<GetMergeOrganisationResponse>
    {
        public int Id { get; set; }
    }
}
