using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.ViewModels.Standard
{
    public class AddStandardOptInConfirmationViewModel
    {
        public string StandardTitle { get; set; }
        public List<string> Versions { get; set; }
        public string FeedbackUrl { get; set; }

        public bool HasMultipleConfirmedVersion
        { 
            get
            {
                return Versions.Count > 0;
            } 
        }

        public string ConfirmedVersions
        {
            get { return string.Join(", ", Versions); }
        }
    }
}
