using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.ViewModels.ApplyForWithdrawal
{
    public class CheckWithdrawalRequestViewModel
    {
        public string StandardName { get; set; }
        public int Level { get; set; }
        public string IFateReferenceNumber { get; set; }
        public string Versions { get; set; }
        public bool InProgressVersionWithdrawals { get; set; }
        public string BackAction { get; set; }
        public string Continue { get; set; }
    }
}
