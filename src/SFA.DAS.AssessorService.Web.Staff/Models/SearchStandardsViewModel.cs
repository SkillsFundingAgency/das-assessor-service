using System.Collections.Generic;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;

namespace SFA.DAS.AssessorService.Web.Staff.Models
{
    public class SearchStandardsViewModel
    {
        public string OrganisationId { get; set; }

        public string OrganisationName { get; set; }
        public string StandardSearchString { get; set; }
        public List<StandardCollation> Results { get; set; }
        public string ErrorMessage { get; set; }
    }

}
