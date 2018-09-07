using SFA.DAS.AssessorService.Api.Types.Models.AO;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.Staff.Models
{
    public class RegisterViewModel
    {
        public string SearchString { get; set; }
        public List<AssessmentOrganisationSummary> Results { get; set; }
        public string ErrorMessage { get; set; }
    }
}