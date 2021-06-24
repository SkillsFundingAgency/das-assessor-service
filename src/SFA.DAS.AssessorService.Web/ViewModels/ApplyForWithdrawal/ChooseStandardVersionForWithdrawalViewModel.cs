using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.ViewModels.ApplyForWithdrawal
{
    public class ChooseStandardVersionForWithdrawalViewModel
    {
        public List<StandardVersion> Versions { get; set; }
        public string WithdrawalType { get; set; }
        public List<string> SelectedVersions { get; set; }
        public bool WholeStandardDisabled { get; set; }

    }
}
