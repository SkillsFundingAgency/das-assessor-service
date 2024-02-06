using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Handlers.OrganisationHandlers
{
    public class GetMergeOrganisationRequest : IRequest<MergeLogEntry>
    {
        public int Id { get; set; }
    }
}
