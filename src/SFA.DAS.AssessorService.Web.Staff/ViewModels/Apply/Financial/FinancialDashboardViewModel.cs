using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Apply.Financial
{
    public class FinancialDashboardViewModel
    {
        public PaginatedList<FinancialApplicationSummaryItem> Applications { get; set; }
    }
}
