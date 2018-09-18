using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Web.Staff.Models
{
    public class BatchSearchViewModel<T>
    {
        public int? BatchNumber { get; set; }
        public int Page { get; set; }
        public PaginatedList<T> PaginatedList { get; set; }
    }
}
