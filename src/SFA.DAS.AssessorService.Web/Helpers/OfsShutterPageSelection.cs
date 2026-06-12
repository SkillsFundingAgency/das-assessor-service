namespace SFA.DAS.AssessorService.Web.Helpers
{
    public class OfsShutterPageSelection
    {
        public bool ShowNeedToRegisterPage { get; init; }
        public bool ShowNeedToSubmitIlrPage { get; init; }
        public OfsShutterPageSelection(bool showNeedToRegisterPage, bool showNeedToSubmitIlrPage)
        {
            ShowNeedToRegisterPage = showNeedToRegisterPage;
            ShowNeedToSubmitIlrPage = showNeedToSubmitIlrPage;
        }
    }
}
