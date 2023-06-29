using SFA.DAS.AssessorService.Web.Helpers;

namespace SFA.DAS.AssessorService.Web.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public int PipelinesCount { get; set; }
        public int AssessmentsCount { get; set; }
        public int StandardsCount { get; set; }
        public Banner Banner { get; set; }
    }
}
