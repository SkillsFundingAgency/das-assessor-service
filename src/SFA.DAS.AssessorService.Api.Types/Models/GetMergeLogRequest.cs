using MediatR;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetMergeLogRequest : IRequest<PaginatedList<MergeLogEntry>>
    {
        public int? PageSize { get; set; }
        public int? PageIndex { get; set; }
        public string OrderBy { get; set; }
        public string OrderDirection { get; set; }
        public string PrimaryEPAOId { get; set; }
        public string SecondaryEPAOId { get; set; }
        public string Status { get; set; }
    }
}
