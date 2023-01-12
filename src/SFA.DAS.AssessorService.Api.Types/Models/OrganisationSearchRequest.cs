using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class OrganisationSearchRequest
    {
        public string SearchString { get; set; }

        public string Name { get; set; }

        public int? Ukprn { get; set; }

        public string Postcode { get; set; }

        public string OrganisationType { get; set; }

        public IEnumerable<OrganisationSearchResult> Organisations { get; set; }

        public IEnumerable<OrganisationType> OrganisationTypes { get; set; }
    }

}
