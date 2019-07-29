using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Web.ViewModels.Organisation
{
    public class OrganisationSearchViewModel
    {
        public string SearchString { get; set; }

        public string Name { get; set; }

        public int? Ukprn { get; set; }

        public string Postcode { get; set; }

        public string OrganisationType { get; set; }

        public int? PageIndex { get; set; }

        [JsonIgnore]
        public PaginatedList<OrganisationSearchResult> Organisations { get; set; }

        [JsonIgnore]
        public IEnumerable<Api.Types.Models.AO.OrganisationType> OrganisationTypes { get; set; }

        public string OrganisationFoundString()
        {
            var result= "0 results found";
            if(Organisations != null && Organisations.Items.Any())
            {
                var resultsString = Organisations.TotalRecordCount > 1 ? "results" : "result";
                result = $"{Organisations.TotalRecordCount} {resultsString} found";
            }
            return result;  
        }
    }
}
