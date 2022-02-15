using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.ViewModels.Apply
{
    public class ApplicationResponseViewModel
    {
        public List<ApplicationResponse> ApplicationResponse { get; set; }
        public bool FinancialInfoStage1Expired { get; set; }
        public string FinancialAssessmentUrl { get; set; }
    }
}
