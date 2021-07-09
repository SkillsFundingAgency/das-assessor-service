using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.ViewModels.ApplyForWithdrawal
{
    public class ReviewStandardVersionsViewModel
    {
        public string StandardName { get; set; }
        public int Level { get; set; }
        public string IFateReferenceNumber { get; set; }
        public List<ReviewStandardVersion> Versions { get; set; }
    }

    public class ReviewStandardVersion
    {
        public string Version { get; set; }
        public bool AbleToWithdraw { get; set; }
    }
}
