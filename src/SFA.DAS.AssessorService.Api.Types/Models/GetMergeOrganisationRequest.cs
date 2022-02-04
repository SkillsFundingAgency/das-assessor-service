using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetMergeOrganisationRequest : IRequest<MergeLogEntry>
    {
        public int Id { get; set; }
    }
}
