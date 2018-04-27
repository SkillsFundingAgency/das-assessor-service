using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public class SearchRequest
    {
        public long Uln { get; set; }
        public string FamilyName { get; set; }
        public List<int> StandardIds { get; set; }
    }
}