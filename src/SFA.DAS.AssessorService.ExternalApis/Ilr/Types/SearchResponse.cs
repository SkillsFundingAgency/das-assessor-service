using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.ExternalApis.Ilr.Types
{
    public class SearchResponse
    {
        public SearchResponse(IEnumerable<IlrRecord> results)
        {
            Results = results;
        }

        public IEnumerable<IlrRecord> Results { get; set; }
    }
}