using SFA.DAS.AssessorService.Api.Types.Models;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.ViewModels.ApplyForWithdrawal
{
    public class ChooseStandardForWithdrawalViewModel
    {
        public List<GetEpaoRegisteredStandardsResponse> Standards { get; set; }
        public int? SelectedStandardForWithdrawal { get; set; }
    }
}
