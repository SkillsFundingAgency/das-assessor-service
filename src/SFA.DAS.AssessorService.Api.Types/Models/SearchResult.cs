using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class SearchResult
    {
        public IEnumerable<Result> Results { get; set; }
    }

    public class Result
    {
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string Uln { get; set; }
        public string DateOfBirth { get; set; }
        public string Standard { get; set; }
        public string LearningStartDate { get; set; }
        public string Provider { get; set; }
    }
}