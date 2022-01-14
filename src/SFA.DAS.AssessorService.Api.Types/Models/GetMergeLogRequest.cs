using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetMergeLogRequest : IRequest<PaginatedList<MergeOrganisation>>
    {
        public int? PageSize { get; set; }
        public int? PageIndex { get; set; }
        public string PrimaryEPAOId { get; set; }
        public string SecondaryEPAOId { get; set; }
    }
}
