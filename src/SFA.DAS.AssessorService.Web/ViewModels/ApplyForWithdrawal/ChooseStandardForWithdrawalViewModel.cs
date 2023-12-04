using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Web.ViewModels.ApplyForWithdrawal
{
    public class ChooseStandardForWithdrawalViewModel
    {
        public PaginatedList<RegisteredStandardsViewModel> Standards { get; set; }
    }
}
