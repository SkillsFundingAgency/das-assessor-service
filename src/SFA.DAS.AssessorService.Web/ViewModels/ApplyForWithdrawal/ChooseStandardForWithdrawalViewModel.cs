using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Web.ViewModels.ApplyForWithdrawal
{
    public class ChooseStandardForWithdrawalViewModel
    {
        public PaginatedList<RegisteredStandardsViewModel> Standards { get; set; }
        public int? SelectedStandardForWithdrawal { get; set; }
    }
}
