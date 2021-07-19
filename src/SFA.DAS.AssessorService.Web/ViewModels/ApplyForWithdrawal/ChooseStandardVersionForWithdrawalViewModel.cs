using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.ViewModels.ApplyForWithdrawal
{
    public class ChooseStandardVersionForWithdrawalViewModel
    {
        public string StandardName { get; set; }
        public int Level { get; set; }
        public string IFateReferenceNumber { get; set; }
        public List<StandardVersion> Versions { get; set; }
        public List<string> SelectedVersions { get; set; }
    }
}
