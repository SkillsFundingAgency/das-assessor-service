using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Web.ViewModels.Organisation
{
    public class OrganisationSearchViewModel
    {
        public string SearchString { get; set; }

        public string Name { get; set; }

        public int? Ukprn { get; set; }

        public string Postcode { get; set; }

        public string OrganisationType { get; set; }

        [JsonIgnore]
        public IEnumerable<OrganisationSearchResult> Organisations { get; set; }

        [JsonIgnore]
        public IEnumerable<OrganisationType> OrganisationTypes { get; set; }
    }
}
