using System;

namespace SFA.DAS.AssessorService.Web.ViewModels.Standard
{
    public class ApplyStandardOfsShutterPageViewModel
    {
        public Guid Id { get; set; }
        public string Search { get; set; }
        public bool ShowNeedToRegisterPage { get; set; }
        public bool ShowNeedToSubmitIlrPage { get; set; }
    }
}
