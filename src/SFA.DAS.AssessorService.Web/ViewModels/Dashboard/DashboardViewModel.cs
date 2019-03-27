namespace SFA.DAS.AssessorService.Web.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public DashboardViewModel(string applyRedirectPath)
        {
            ApplyRedirectPath = applyRedirectPath;
        }

        public int PipelinesCount { get; set; }
        public int AssessmentsCount { get; set; }
        public int StandardsCount { get; set; }
        public string ApplyRedirectPath { get; }

    }
}
