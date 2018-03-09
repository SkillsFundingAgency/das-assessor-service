using System.Collections.Generic;

namespace SFA.DAS.AssessorService.ExternalApis.Ilr.Types
{
    public class IlrSearchRequest
    {
        public string Uln { get; set; }
        public string Surname { get; set; }
        public IEnumerable<string> StandardIds { get; set; }
    }
}