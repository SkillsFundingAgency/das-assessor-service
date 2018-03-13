using System.Collections.Generic;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Web.ViewModels.Search
{
    public class SearchViewModel
    {
        public string Uln { get; set; }
        public string Surname { get; set; }
        public IEnumerable<ResultViewModel> SearchResults { get; set; }
    }
}