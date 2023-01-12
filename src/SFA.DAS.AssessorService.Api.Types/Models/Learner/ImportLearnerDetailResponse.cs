using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class ImportLearnerDetailResponse
    {
        public List<ImportLearnerDetailResult> LearnerDetailResults { get; set; }
    }

    public class ImportLearnerDetailResult
    {
        public long? Uln { get; set; }

        public int? StdCode { get; set; }

        public string Outcome { get; set; }

        public List<string> Errors { get; set; }
    }
}