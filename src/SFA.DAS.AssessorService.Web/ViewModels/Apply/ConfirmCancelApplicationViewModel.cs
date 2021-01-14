using System;

namespace SFA.DAS.AssessorService.Web.ViewModels.Apply
{
    public class ConfirmCancelApplicationViewModel
    {
        public Guid Id { get; set; }
        public string AreYouSure { get; set; }
        public string StandardWithReference { get; set; }
    }
}
