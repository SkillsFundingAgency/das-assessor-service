using System.Collections.Generic;
using SFA.DAS.Apprenticeships.Api.Types;

namespace SFA.DAS.AssessorService.Web.Staff.Models
{
    public class StandardsViewModel
    {
        public string StandardSearchString { get; set; }
        public List<StandardSummary> Results { get; set; }
        public string ErrorMessage { get; set; }
    }

}
